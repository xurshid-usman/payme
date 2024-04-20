using Microsoft.EntityFrameworkCore;
using Payme.Data;
using Payme.Entities;
using Payme.Exceptions;
using Payme.Extensions;

namespace Payme.Features;

public class PerformTransaction(AppDbContext db)
{
    public record Request(string Id);

    public record Result(string Transaction, long PerformTime, OrderTransactionState State);


    public async Task<Result> Handle(Request req, CancellationToken ct)
    {
        var transaction = await db.OrderTransactions
            .Where(w => w.GatewayTransactionId == req.Id)
            .FirstOrDefaultAsync(ct);

        if (transaction == null) throw new TransactionNotFoundException();

        if (transaction.State == OrderTransactionState.Complete)
        {
            if (transaction.PerformTime == null) 
                throw new DomainException("Perform time must be not null");
            
            return new Result(transaction.Id + "", transaction.PerformTime.Value.GetUnixTicks(), transaction.State);
        }

        if (transaction.State == OrderTransactionState.Canceled
            || transaction.State == OrderTransactionState.CanceledAfterComplete)
        {
            throw new TransactionFoundButCancelledException();
        }

        if (transaction.IsExpired())
        {
            transaction.Cancel(OrderTransactionCancelReason.TransactionTimeout);
            await db.SaveChangesAsync(ct);
            
            throw new TransactionExpiredException();
        }
        
        var order = await db.Orders
            .Where(w => w.Id == transaction.OrderId)
            .FirstOrDefaultAsync(ct);
        
        if (order == null) throw new OrderNotExistsException();
        if (order.Amount != transaction.Amount) throw new WrongAmountException();
        if (order.State != OrderState.WaitingPay) throw new InvalidOrderStateException();

        order.AcceptPay();
        transaction.Complete();

        await db.SaveChangesAsync(ct);

        if (transaction.PerformTime == null) 
            throw new DomainException("Perform time must be not null");
        
        return new Result(transaction.Id + "", transaction.PerformTime.Value.GetUnixTicks(), transaction.State);
    }

    private Task<bool> ExistsAnyActiveTransaction(long orderId, CancellationToken ct)
    {
        var transactions = from item in db.OrderTransactions
            where item.OrderId == orderId
                  && (item.State == OrderTransactionState.InProgress || item.State == OrderTransactionState.Complete)
            select item;

        return transactions.AnyAsync(ct);
    }
}
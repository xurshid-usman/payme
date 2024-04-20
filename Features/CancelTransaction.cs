using Microsoft.EntityFrameworkCore;
using Payme.Data;
using Payme.Entities;
using Payme.Exceptions;
using Payme.Extensions;

namespace Payme.Features;

public class CancelTransaction(AppDbContext db)
{
    public record Request(string Id, OrderTransactionCancelReason Reason);

    public record Result(string Transaction, long CancelTime, OrderTransactionState State);


    public async Task<Result> Handle(Request req, CancellationToken ct)
    {
        var transaction = await db.OrderTransactions
            .Where(w => w.GatewayTransactionId == req.Id)
            .FirstOrDefaultAsync(ct);

        if (transaction == null) throw new TransactionNotFoundException();

        if (transaction.State == OrderTransactionState.InProgress)
        {
            transaction.Cancel(req.Reason);
            
            if (transaction.CancelTime == null) 
                throw new DomainException("Cancel time must be not null");

            await db.SaveChangesAsync(ct);
            
            return new Result(transaction.Id + "", transaction.CancelTime.Value.GetUnixTicks(), transaction.State);
        }

        if (transaction.State == OrderTransactionState.Canceled
            || transaction.State == OrderTransactionState.CanceledAfterComplete)
        {
            if (transaction.CancelTime == null) 
                throw new DomainException("Cancel time must be not null");
            
            return new Result(transaction.Id + "", transaction.CancelTime.Value.GetUnixTicks(), transaction.State);
        }
        
        // if transaction complete
        
        var order = await db.Orders
            .Where(w => w.Id == transaction.OrderId)
            .FirstOrDefaultAsync(ct);
        
        if (order == null) throw new OrderNotExistsException();

        if (order.State == OrderState.PayAccepted)
        {
            //TODO: check order for cancelling
            order.Cancel();
            //throw new UnableCancelTransactionException();
        }
        
        if (order.State == OrderState.Available
            || order.State == OrderState.WaitingPay)
        {
            order.Cancel();
        }

        transaction.Cancel(req.Reason);

        await db.SaveChangesAsync(ct);

        if (transaction.CancelTime == null) 
            throw new DomainException("Perform time must be not null");
        
        return new Result(transaction.Id + "", transaction.CancelTime.Value.GetUnixTicks(), transaction.State);
    }
}
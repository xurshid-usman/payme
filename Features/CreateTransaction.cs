using Microsoft.EntityFrameworkCore;
using Payme.Data;
using Payme.Entities;
using Payme.Exceptions;
using Payme.Extensions;
using Payme.Types;

namespace Payme.Features;

public class CreateTransaction(AppDbContext db)
{
    public record Request(string Id, long Time, int Amount, Account Account);
    public record Result(long CreateTime, string Transaction, OrderTransactionState State, Receiver[]? Receivers = null);
    public record Receiver(string Id, int Amount);
    
    
    public async Task<Result> Handle(Request req, CancellationToken ct)
    {
       var transaction = await db.OrderTransactions
            .Where(w => w.GatewayTransactionId == req.Id)
            .FirstOrDefaultAsync(ct);

        if (transaction != null)
        {
            if (transaction.IsExpired())
            {
                transaction.Cancel(OrderTransactionCancelReason.TransactionTimeout);
                await db.SaveChangesAsync(ct);
                throw new TransactionExpiredException();
            }
            
            if (transaction.State == OrderTransactionState.InProgress)
            {
                return new Result(transaction.CreateTime.GetUnixTicks(), transaction.Id + "", transaction.State);
            }

            throw new TransactionFoundButCancelledException();
        }
        
        //CheckPerformTransaction
        if (req.Amount < 500_00) throw new WrongAmountException();
        if (!long.TryParse(req.Account.OrderId, out var orderId)) throw new AccountException();

        var order = await db.Orders
            .Where(w => w.Id == orderId)
            .FirstOrDefaultAsync(ct);
        
        if (order == null) throw new OrderNotExistsException();
        if (req.Amount != order.Amount) throw new WrongAmountException();
        if (order.State != OrderState.Available) throw new InvalidOrderStateException();

        if(await ExistsAnyActiveTransaction(orderId, ct))
            throw new ExistsActiveTransactionException();
        //end

        if (DateTime.Now.GetUnixTicks() - req.Time > OrderTransaction.Timeout) 
            throw new TransactionTimeOutException();
        
        // set receivers if needed
        
        transaction = new OrderTransaction(orderId, req.Id, req.Time, req.Amount);
        await db.OrderTransactions.AddAsync(transaction, ct);

        order.WaitPay();
        
        await db.SaveChangesAsync(ct);

        return new Result(transaction.CreateTime.GetUnixTicks(), transaction.Id + "", transaction.State);
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
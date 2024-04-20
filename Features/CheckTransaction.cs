using Microsoft.EntityFrameworkCore;
using Payme.Data;
using Payme.Entities;
using Payme.Exceptions;
using Payme.Extensions;

namespace Payme.Features;

public class CheckTransaction(AppDbContext db)
{
    public record Request(string Id);

    public record Result(long CreateTime, 
        long PerformTime, 
        long CancelTime, 
        string Transaction, 
        OrderTransactionState State,
        OrderTransactionCancelReason? Reason);


    public async Task<Result> Handle(Request req, CancellationToken ct)
    {
        var transaction = await db.OrderTransactions
            .Where(w => w.GatewayTransactionId == req.Id)
            .FirstOrDefaultAsync(ct);

        if (transaction == null) throw new TransactionNotFoundException();
        
        return new Result(transaction.CreateTime.GetUnixTicks(),
            transaction.PerformTime.HasValue ? transaction.PerformTime.Value.GetUnixTicks() : 0,
            transaction.CancelTime.HasValue ? transaction.CancelTime.Value.GetUnixTicks() : 0,
            transaction.Id + "", 
            transaction.State,
            transaction.Reason);
    }
}
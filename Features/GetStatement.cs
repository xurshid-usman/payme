using Microsoft.EntityFrameworkCore;
using Payme.Data;
using Payme.Entities;
using Payme.Extensions;
using Payme.Types;

namespace Payme.Features;

public class GetStatement(AppDbContext db)
{
    public record Request(long From, long To);

    public record Result(Item[] Transactions);

    public record Item(
        string Id,
        long Time,
        int Amount,
        Account Account,
        long CreateTime,
        long PerformTime,
        long CancelTime,
        string Transaction,
        OrderTransactionState State,
        OrderTransactionCancelReason? Reason,
        Receiver[]? Receivers = null);

    public record Receiver(string Id, int Amount);


    public async Task<Result> Handle(Request req, CancellationToken ct)
    {
        var items = from item in db.OrderTransactions
            
            where item.GatewayTime >= req.From 
                && item.GatewayTime <= req.To
            
            select new Item(item.GatewayTransactionId,
                item.GatewayTime,
                item.Amount,
                new Account(item.OrderId + ""),
                item.CreateTime.GetUnixTicks(),
                item.PerformTime.HasValue ? item.PerformTime.Value.GetUnixTicks() : 0,
                item.CancelTime.HasValue ? item.CancelTime.Value.GetUnixTicks() : 0,
                item.Id + "",
                item.State,
                item.Reason,
                null);

        var transactions = await items.ToListAsync(ct);

        return new Result(transactions.ToArray());
    }
}
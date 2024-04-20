using Microsoft.EntityFrameworkCore;
using Payme.Data;
using Payme.Entities;
using Payme.Exceptions;
using Payme.Types;

namespace Payme.Features;

public class CheckPerformTransaction(AppDbContext db)
{
    public record Request(int Amount, Account Account);

    public record Result(bool Allow, Detail Detail);

    public record Detail(int ReceiptType, List<Item> Items);

    public record Item(
        string Title,
        int Price,
        int Count,
        int Discount,
        string? Code,
        string? PackageCode,
        int VatPercent);

    public async Task<Result> Handle(Request req, CancellationToken ct)
    {
        if (!long.TryParse(req.Account.OrderId, out var orderId)) throw new AccountException();

        var order = await db.Orders
            .Where(w => w.Id == orderId)
            .FirstOrDefaultAsync(ct);

        if (order == null) throw new OrderNotExistsException();
        if (order.State != OrderState.Available) throw new InvalidOrderStateException();
        if (order.Amount != req.Amount) throw new WrongAmountException();
        
        var transactions = from item in db.OrderTransactions
            where item.OrderId == orderId
                  && (item.State == OrderTransactionState.InProgress || item.State == OrderTransactionState.Complete)
            select item;

        if (await transactions.AnyAsync(ct)) 
            throw new ExistsActiveTransactionException();

        var items = from item in db.OrderItems
            where item.OrderId == orderId
            select new Item(item.Title,
                item.Price,
                item.Count,
                item.Discount,
                item.Code ?? "",
                item.PackageCode ?? "",
                item.VatPercent);

        var details = new Detail(0, await items.ToListAsync(ct));

        return new Result(true, details);
    }
}
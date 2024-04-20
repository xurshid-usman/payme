using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Payme.Data;
using Payme.Exceptions;

namespace Payme.Features;

public class SetFiscalData(AppDbContext db)
{
    /// <summary>
    /// SetFiscalData request
    /// </summary>
    /// <param name="Id">Gateway transaction identifier</param>
    /// <param name="Type">PERFORM, CANCEL</param>
    public record Request(string Id, string Type, FiscalData FiscalData);
    public record FiscalData(int ReceiptType, List<Item> Items);
    public record Item(
        string ReceiptId,
        int StatusCode,
        string Message,
        string TerminalId,
        string FiscalSign,
        string QrCodeUrl,
        string Date);
    
    public record Result(bool Success);

    public async Task<Result> Handle(Request req, CancellationToken ct)
    {
        var transaction = await db.OrderTransactions
            .Where(w => w.GatewayTransactionId == req.Id)
            .FirstOrDefaultAsync(ct);

        if (transaction == null) throw new TransactionNotFoundException();

        if (req.Type == "PERFORM") 
            transaction.SetPerformFiscalData(JsonSerializer.Serialize(req.FiscalData));
        else 
            transaction.SetCancelFiscalData(JsonSerializer.Serialize(req.FiscalData));
        
        await db.SaveChangesAsync(ct);
        
        return new Result(true);
    }
}
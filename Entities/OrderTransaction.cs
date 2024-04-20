using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payme.Extensions;
using Payme.Features;
using Payme.Types;

namespace Payme.Entities;

public class OrderTransaction
{
    // Transaction expiration time in milliseconds. 43 200 000 ms = 12 hours.
    public const long Timeout = 43200000;
    
    private OrderTransaction() { }

    public OrderTransaction(long orderId
        , string gatewayTransactionId
        , long gatewayTime
        , int amount)
    {
        OrderId = orderId;
        GatewayTransactionId = gatewayTransactionId;
        GatewayTime = gatewayTime;
        CreateTime = DateTime.Now;
        State = OrderTransactionState.InProgress;
        Amount = amount;
    }

    public long Id { get; private set; }
    public long OrderId { get; private set; }

    public string GatewayTransactionId { get; private set; } = null!;
    public long GatewayTime { get; private set; }
    
    public DateTime CreateTime { get; private set; }
    public DateTime? PerformTime { get; private set; }
    public DateTime? CancelTime { get; private set; }


    public OrderTransactionState State { get; private set; }

    public OrderTransactionCancelReason? Reason { get; private set; }
    
    public int Amount { get; private set; }
    
    public string? PerformFiscalData { get; private set; }
    public string? CancelFiscalData { get; private set; }

    public bool IsExpired()
    {
        return State == OrderTransactionState.InProgress
            && (DateTime.Now.GetUnixTicks() - CreateTime.GetUnixTicks()) > Timeout;
    }

    public void Cancel(OrderTransactionCancelReason reason)
    {
        CancelTime = DateTime.Now;

        State = State == OrderTransactionState.Complete
            ? OrderTransactionState.CanceledAfterComplete
            : OrderTransactionState.Canceled;

        Reason = reason;
    }

    public void Complete()
    {
        PerformTime = DateTime.Now;
        State = OrderTransactionState.Complete;
    }

    public void SetPerformFiscalData(string fiscalData)
    {
        PerformFiscalData = fiscalData;
    }
    
    public void SetCancelFiscalData(string fiscalData)
    {
        CancelFiscalData = fiscalData;
    }
}

public enum OrderTransactionState
{
    InProgress = 1,
    Complete = 2,
    
    Canceled = -1,
    CanceledAfterComplete = -2
}

public enum OrderTransactionCancelReason : int
{
    ReceiverNotFound = 1,
    DebitOperationError = 2,
    TransactionError = 3,
    TransactionTimeout = 4,
    MoneyBack = 5,
    UnknownError = 10
}

public class OrderTransactionConfiguration : IEntityTypeConfiguration<OrderTransaction>
{
    public void Configure(EntityTypeBuilder<OrderTransaction> builder)
    {
        builder
            .HasKey(key => key.Id);

        builder.HasIndex(i => i.GatewayTransactionId);

        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(24);
        
        builder.Property(p => p.CreateTime)
            .HasColumnType("timestamp with time zone");
        
        builder.Property(p => p.PerformTime)
            .HasColumnType("timestamp with time zone");
        
        builder.Property(p => p.CancelTime)
            .HasColumnType("timestamp with time zone");
    }
}
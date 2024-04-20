using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payme.Exceptions;

namespace Payme.Entities;

public class Order
{
    public long Id { get; private set; }
    public int Amount { get; private set; }
    public OrderState State { get; private set; }

    public void WaitPay()
    {
        if (State != OrderState.Available) 
            throw new DomainException("Invalid order state.");
        
        State = OrderState.WaitingPay;
    }

    public void AcceptPay()
    {
        if (State != OrderState.WaitingPay) 
            throw new DomainException("Invalid order state.");
        
        State = OrderState.PayAccepted;
    }

    public void Cancel()
    {
        // if (State == OrderState.PayAccepted) 
        //     throw new DomainException("Invalid order state.");
        
        State = OrderState.Cancelled;
    }
}

public enum OrderState
{
    // Order is available for payment.
    Available = 0,

    // Pay in progress, order must not be changed.
    WaitingPay = 1,

    // Order completed and not available for payment.
    PayAccepted = 4,

    // Order is cancelled.
    Cancelled = 50
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasKey(key => key.Id);

        builder
            .HasMany<OrderTransaction>()
            .WithOne()
            .HasForeignKey(k => k.OrderId)
            .IsRequired();
    }
}
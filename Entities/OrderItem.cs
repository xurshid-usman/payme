using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payme.Entities;

public class OrderItem
{
    public long Id { get; private set; }
    public long OrderId { get; set; }
    /// <summary>
    /// Mahsulot yoki xizmat nomi
    /// </summary>
    public string Title { get; private set; } = null!;
    /// <summary>
    /// Narx, tiyinlarda
    /// </summary>
    public int Price { get; private set; }
    /// <summary>
    /// Miqdor
    /// </summary>
    public int Count { get; private set; }
    /// <summary>
    /// Count hisobga olingan chegirma, tiyinlarda
    /// </summary>
    public int Discount { get; private set; }
    /// <summary>
    /// MXIK
    /// </summary>
    public string? Code { get; private set; }
    /// <summary>
    /// Mahsulot yoki xizmat o'lchov birligi kodi
    /// </summary>
    public string? PackageCode { get; private set; }
    /// <summary>
    /// QQS foizi
    /// </summary>
    public int VatPercent { get; private set; }
}


public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .HasKey(key => key.Id);
        
        builder
            .HasMany<OrderItem>()
            .WithOne()
            .HasForeignKey(k=>k.OrderId)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(250);
        
        builder.Property(p => p.Code)
            .HasMaxLength(32);
        
        builder.Property(p => p.PackageCode)
            .HasMaxLength(32);
    }
}
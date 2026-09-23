using Asisya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asisya.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.ProductId);
        builder.Property(p => p.ProductId).HasColumnName("product_id");

        builder.Property(p => p.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.SupplierId).HasColumnName("supplier_id");
        builder.Property(p => p.CategoryId).HasColumnName("category_id").IsRequired();

        builder.Property(p => p.QuantityPerUnit).HasColumnName("quantity_per_unit").HasMaxLength(50);
        builder.Property(p => p.UnitPrice).HasColumnName("unit_price").HasColumnType("numeric(18,2)").HasDefaultValue(0.00m);
        builder.Property(p => p.UnitsInStock).HasColumnName("units_in_stock").HasDefaultValue(0);
        builder.Property(p => p.UnitsOnOrder).HasColumnName("units_on_order").HasDefaultValue(0);
        builder.Property(p => p.ReorderLevel).HasColumnName("reorder_level").HasDefaultValue(0);
        builder.Property(p => p.Discontinued).HasColumnName("discontinued").HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relaciones y Llaves Foráneas
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(p => p.CategoryId).HasDatabaseName("idx_products_category_id");
        builder.HasIndex(p => p.ProductName).HasDatabaseName("idx_products_product_name");
        builder.HasIndex(p => p.UnitPrice).HasDatabaseName("idx_products_unit_price");
    }
}

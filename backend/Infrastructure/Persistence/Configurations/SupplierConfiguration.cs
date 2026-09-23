using Asisya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asisya.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasKey(s => s.SupplierId);
        builder.Property(s => s.SupplierId).HasColumnName("supplier_id");

        builder.Property(s => s.CompanyName)
            .HasColumnName("company_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.ContactName).HasColumnName("contact_name").HasMaxLength(100);
        builder.Property(s => s.ContactTitle).HasColumnName("contact_title").HasMaxLength(50);
        builder.Property(s => s.Address).HasColumnName("address").HasMaxLength(255);
        builder.Property(s => s.City).HasColumnName("city").HasMaxLength(50);
        builder.Property(s => s.Region).HasColumnName("region").HasMaxLength(50);
        builder.Property(s => s.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
        builder.Property(s => s.Country).HasColumnName("country").HasMaxLength(50);
        builder.Property(s => s.Phone).HasColumnName("phone").HasMaxLength(30);
        builder.Property(s => s.Fax).HasColumnName("fax").HasMaxLength(30);
        builder.Property(s => s.HomePage).HasColumnName("home_page");
    }
}

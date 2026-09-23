using Asisya.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asisya.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.CategoryId).HasColumnName("category_id");

        builder.Property(c => c.CategoryName)
            .HasColumnName("category_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => c.CategoryName).IsUnique();

        builder.Property(c => c.Description)
            .HasColumnName("description");

        builder.Property(c => c.Picture)
            .HasColumnName("picture");
    }
}

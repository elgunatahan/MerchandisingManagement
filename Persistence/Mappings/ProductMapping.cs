using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(e => e.Id);

            builder.Property(p => p.CategoryId).HasColumnType("int").IsRequired(false);
            builder.Property(p => p.Description).HasColumnType("nvarchar(1024)");
            builder.Property(p => p.IsActive).HasColumnType("bit").IsRequired();
            builder.Property(p => p.StockQuantity).HasColumnType("int").IsRequired();
            builder.Property(p => p.Title).HasColumnType("nvarchar(200)").IsRequired();

            builder.Property(p => p.CreatedAt).HasColumnType("datetime2").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.UpdatedAt).HasColumnType("datetime2");
            builder.Property(p => p.UpdatedBy).HasColumnType("nvarchar(256)");

            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.Property(p => p.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();

            builder.HasOne(m => m.Category)
                .WithMany()
                .HasForeignKey(m => m.CategoryId);
        }
    }
}

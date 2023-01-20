using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
    public class CategoryMapping : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(e => e.Id);
            builder.Property(p => p.Name).HasColumnType("nvarchar(200)").IsRequired();

            builder.Property(p => p.CreatedAt).HasColumnType("datetime").IsRequired();
            builder.Property(p => p.CreatedBy).HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(p => p.UpdatedAt).HasColumnType("datetime");
            builder.Property(p => p.UpdatedBy).HasColumnType("nvarchar(256)");

            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.Property(p => p.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
        }
    }
}

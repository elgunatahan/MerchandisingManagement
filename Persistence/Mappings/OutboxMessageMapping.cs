using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
    public class OutboxMessageMapping : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(e => e.Id);

            builder.Property(p => p.OccurredOn).HasColumnType("datetime2(7)").IsRequired();
            builder.Property(p => p.Status).HasColumnType("int").IsRequired().HasDefaultValue(OutboxMessageStatus.New);
            builder.Property(p => p.Type).HasColumnType("nvarchar(255)").IsRequired();
            builder.Property(p => p.Data).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(p => p.RoutingKey).HasColumnType("nvarchar(255)");
            builder.Property(p => p.ProcessedDate).HasColumnType("datetime2(7)");
            builder.Property(p => p.QueueName).HasColumnType("nvarchar(1024)");
        }
    }
}

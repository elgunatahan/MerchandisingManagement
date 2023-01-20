using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence
{
    public class MerchandisingManagementContext : DbContext, IMerchandisingManagementContext
    {
        public MerchandisingManagementContext(DbContextOptions<MerchandisingManagementContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.EnableAutoHistory(changedMaxLength: null);
            builder.ApplyConfigurationsFromAssembly(typeof(MerchandisingManagementContext).Assembly);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return Database.CreateExecutionStrategy();
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}
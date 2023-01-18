using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
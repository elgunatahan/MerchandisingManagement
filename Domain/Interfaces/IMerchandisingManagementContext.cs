using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.Interfaces
{
    public interface IMerchandisingManagementContext
    {
        DbSet<Category> Categories { get; set; }

        DbSet<OutboxMessage> OutboxMessages { get; set; }
        
        DbSet<Product> Products { get; set; }
        
        IExecutionStrategy CreateExecutionStrategy();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

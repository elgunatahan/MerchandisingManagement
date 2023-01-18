using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductService
    {
        Task<bool> DecideProductActivity(Product product, CancellationToken cancellationToken);
    }
}

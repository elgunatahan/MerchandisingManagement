using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IMerchandisingManagementContext _merchandisingManagementContext;

        public ProductService(IMerchandisingManagementContext merchandisingManagementContext)
        {
            _merchandisingManagementContext = merchandisingManagementContext;
        }

        public async Task<bool> DecideProductActivity(Product product, CancellationToken cancellationToken)
        {
            if(!product.CategoryId.HasValue) return false;

            Category category = await _merchandisingManagementContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == product.CategoryId, cancellationToken);

            if(category == null)
            {
                ProblemDetails problemDetails = new ProblemDetails()
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Category Not Found",
                    Type = "category-not-found",
                    Detail = "Category not found",
                    Extensions = { new KeyValuePair<string, object>("CategoryId", product.CategoryId) }
                };

                throw new ProblemDetailsException(problemDetails);
            }

            if (product.StockQuantity < category.MinStockQuantity) return false;

            return true;
        }
    }
}

using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Queries.SearchProduct
{
    public class SearchProductQueryHandler : IRequestHandler<SearchProductQuery, SearchProductRepresentation>
    {
        private readonly IMerchandisingManagementContext _merchandisingManagementContext;

        public SearchProductQueryHandler(IMerchandisingManagementContext merchandisingManagementContext)
        {
            _merchandisingManagementContext = merchandisingManagementContext;
        }

        public async Task<SearchProductRepresentation> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _merchandisingManagementContext.Products.Include(x => x.Category).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x => x.Title.Contains(request.Keyword) || 
                                 x.Description.Contains(request.Keyword) || 
                                 x.Category.Name.Contains(request.Keyword));
            }

            if (request.MinStockQuantity.HasValue)
            {
                query = query.Where(x => x.StockQuantity >= request.MinStockQuantity);
            }

            if (request.MaxStockQuantity.HasValue)
            {
                query = query.Where(x => x.StockQuantity <= request.MaxStockQuantity);
            }

            OrderBy orderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? OrderBy.CreatedAt : Enum.Parse<OrderBy>(request.OrderBy);
            OrderDirection order = string.IsNullOrWhiteSpace(request.Order) ? OrderDirection.Descending : Enum.Parse<OrderDirection>(request.Order);

            if (order == OrderDirection.Ascending)
            {
                query = query.OrderBy(orderBy.ToString());
            }
            else
            {
                query = query.OrderBy(orderBy + " descending");
            }

            int skip = (request.Page - 1) * request.PageSize;

            int take = request.PageSize;

            int totalItemCount = await query.CountAsync();

            List<Product> products = await query.Skip(skip).Take(take).ToListAsync();

            return new SearchProductRepresentation()
            {
                Page = request.Page,
                TotalCount = totalItemCount,
                SearchProductVMs = products.Select(x=> new SearchProductVM()
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category?.Name,
                    CreatedAt = x.CreatedAt,
                    Description = x.Description,
                    Id = x.Id,
                    IsActive = x.IsActive,
                    StockQuantity = x.StockQuantity,
                    Title = x.Title,
                    UpdatedAt = x.UpdatedAt
                }).ToList()
            };
        }
    }
}

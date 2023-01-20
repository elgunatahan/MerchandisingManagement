using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdRepresentation>
    {
        private readonly IMerchandisingManagementContext _merchandisingManagementContext;

        public GetProductByIdQueryHandler(IMerchandisingManagementContext merchandisingManagementContext)
        {
            _merchandisingManagementContext = merchandisingManagementContext;
        }

        public async Task<GetProductByIdRepresentation> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {

            Product product = await _merchandisingManagementContext.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
            {
                ProblemDetails problemDetails = new ProblemDetails()
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Product Not Found",
                    Type = "product-not-found",
                    Detail = "Product not found with given Id",
                    Extensions = { new KeyValuePair<string, object>("ProductId", request.Id) }
                };

                throw new ProblemDetailsException(problemDetails);
            }

            return new GetProductByIdRepresentation()
            {
                Id = product.Id,
                Title = product.Title,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                Description = product.Description,
                IsActive = product.IsActive,
                StockQuantity = product.StockQuantity,
                UpdatedAt = product.UpdatedAt
            };


        }
    }
}

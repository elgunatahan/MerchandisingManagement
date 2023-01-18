using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IMerchandisingManagementContext _merchandisingManagementContext;
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IMerchandisingManagementContext merchandisingManagementContext, IProductService productService)
        {
            _merchandisingManagementContext = merchandisingManagementContext;
            _productService = productService;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = await _merchandisingManagementContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

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

            product.Update(request.CategoryId, request.StockQuantity, request.Description);

            bool isActive = await _productService.DecideProductActivity(product, cancellationToken);
            product.SetIsActive(isActive);

            await _merchandisingManagementContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

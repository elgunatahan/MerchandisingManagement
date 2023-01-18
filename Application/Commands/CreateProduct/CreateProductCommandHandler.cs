using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductRepresentation>
    {
        private readonly IMerchandisingManagementContext _merchandisingManagementContext;
        private readonly IProductService _productService;

        public CreateProductCommandHandler(IMerchandisingManagementContext merchandisingManagementContext, IProductService productService)
        {
            _merchandisingManagementContext = merchandisingManagementContext;
            _productService = productService;
        }
        public async Task<CreateProductRepresentation> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            bool isProductAlreadyExist = await _merchandisingManagementContext.Products.AnyAsync(x => x.Title.Equals(request.Title), cancellationToken);

            if (isProductAlreadyExist)
            {
                ProblemDetails problemDetails = new ProblemDetails()
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Product has already exist.",
                    Type = "product-already-exist",
                    Detail = "There is already product record for the title"
                };

                throw new ProblemDetailsException(problemDetails);
            }

            Product product = new Product(request.CategoryId, request.Description, request.StockQuantity, request.Title);

            bool isActive = await _productService.DecideProductActivity(product, cancellationToken);

            product.SetIsActive(isActive);

            _merchandisingManagementContext.Products.Add(product);

            await _merchandisingManagementContext.SaveChangesAsync(cancellationToken);

            return new CreateProductRepresentation { Id = product.Id };
        }
    }
}

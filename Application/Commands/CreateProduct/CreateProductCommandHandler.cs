using Domain.Common.Factories;
using Domain.Common.Filters;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace Application.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductRepresentation>
    {
        private readonly IOutboxMessageFactory _outboxMessageFactory;
        private readonly IMerchandisingManagementContext _merchandisingManagementContext;
        private readonly IProductService _productService;

        public CreateProductCommandHandler(IOutboxMessageFactory outboxMessageFactory, IMerchandisingManagementContext merchandisingManagementContext, IProductService productService)
        {
            _outboxMessageFactory = outboxMessageFactory;
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

            IExecutionStrategy executionStrategy = _merchandisingManagementContext.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _merchandisingManagementContext.SaveChangesAsync(cancellationToken);

                    ProductCreated productCreated = new ProductCreated()
                    {
                        Id = product.Id,
                    }; 
                    
                    OutboxMessage outboxMessage = _outboxMessageFactory.From(productCreated, product.CreatedAt);

                    _merchandisingManagementContext.OutboxMessages.Add(outboxMessage);

                    await _merchandisingManagementContext.SaveChangesAsync(cancellationToken);

                    scope.Complete();
                }
            });
           
            return new CreateProductRepresentation { Id = product.Id };
        }
    }
}

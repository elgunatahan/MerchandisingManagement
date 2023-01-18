using Application.Queries.GetProductById;
using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistence;

namespace Application.UnitTests.Queries.GetCampaignOfferById
{
    public class GetProductByIdQueryTests
    {
        private GetProductByIdQueryHandler handler;
        private Task<GetProductByIdRepresentation> result;

        [Test]
        public async Task returned_product_must_be_returned_correctly()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);

            Product product = new Product(1, "test description", 100, "test title");
            context.Products.Add(product);

            await context.SaveChangesAsync(CancellationToken.None);

            handler = new GetProductByIdQueryHandler(context);

            result = handler.Handle(new GetProductByIdQuery { Id = product.Id }, CancellationToken.None);

            result.Result.Id.Should().Be(product.Id);
            result.Result.Title.Should().Be(product.Title);
            result.Result.StockQuantity.Should().Be(product.StockQuantity);
            result.Result.Description.Should().Be(product.Description);
        }

        [Test]
        public async Task it_should_throw_when_product_not_found()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);

            handler = new GetProductByIdQueryHandler(context);

            Func<Task> action = async () => await handler.Handle(new GetProductByIdQuery { Id = 1 }, CancellationToken.None);

            var exception = await action.Should().ThrowAsync<ProblemDetailsException>();
            exception.And.Value.Type.Should().Be("product-not-found");
            exception.And.Value.Status.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}

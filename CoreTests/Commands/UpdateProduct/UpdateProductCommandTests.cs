using Application.Commands.UpdateProduct;
using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Persistence;

namespace CoreTests.Commands.UpdateProduct
{
    public class UpdateProductCommandTests
    {
        private UpdateProductCommandHandler handler;

        [Test]
        public async Task it_should_throw_when_product_not_found()
        {
            int notExistingProductId = 123;

            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                                  .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);
            Mock<IProductService> productService = new Mock<IProductService>();

            handler = new UpdateProductCommandHandler(context, productService.Object);

            Func<Task> action = async () => await handler.Handle(new UpdateProductCommand { Id = notExistingProductId }, CancellationToken.None);

            var exception = await action.Should().ThrowAsync<ProblemDetailsException>();
            exception.And.Value.Type.Should().Be("product-not-found");
            exception.And.Value.Status.Should().Be(StatusCodes.Status404NotFound);
        }

        [Test]
        public async Task successfully_update_product_with_is_active_status_false()
        {
            string givenTitle = "new test title";
            string givenCategoryName = "test category name";

            string descriptionBefore = "test description before";
            int categoryIdBefore = 1;
            int stockQuantityBefore = 100;

            string descriptionAfter = "test description after";
            int categoryIdAfter = 20;
            int stockQuantityAfter = 150;

            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);
            Mock<IProductService> productService = new Mock<IProductService>();



            Product product = new Product(categoryIdBefore, descriptionBefore, stockQuantityBefore, givenTitle);
            context.Products.Add(product);

            Category category = new Category(categoryIdAfter, givenCategoryName);
            context.Categories.Add(category);

            await context.SaveChangesAsync(CancellationToken.None);

            productService
                .Setup(x => x.DecideProductActivity(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            handler = new UpdateProductCommandHandler(context, productService.Object);


            var result = await handler.Handle(new UpdateProductCommand
            {
                Id = product.Id,
                CategoryId = categoryIdAfter,
                Description = descriptionAfter,
                StockQuantity = stockQuantityAfter
            }, CancellationToken.None);

            result.Should().NotBeNull();

            var updatedItem = context.Products.First(x => x.Title.Equals(givenTitle));
            updatedItem.IsActive.Should().BeTrue();
            updatedItem.CategoryId.Should().Be(categoryIdAfter);
            updatedItem.StockQuantity.Should().Be(stockQuantityAfter);
            updatedItem.Description.Should().Be(descriptionAfter);
        }
    }
}

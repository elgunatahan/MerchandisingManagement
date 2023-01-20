using Application.Commands.CreateProduct;
using Domain.Common.Factories;
using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Persistence;

namespace CoreTests.Commands.CreateProduct
{
    public class CreateProductCommandTests
    {
        private CreateProductCommandHandler handler;
        private const string categoryName = "test-category-name";
        [Test]
        public async Task it_should_throw_when_product_already_exist()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<IOutboxMessageFactory> outboxMessageFactory = new Mock<IOutboxMessageFactory>();


            string givenTitle = "test title";
            Product product = new Product(1, "test description", 100, givenTitle);
            context.Products.Add(product);

            await context.SaveChangesAsync(CancellationToken.None);

            handler = new CreateProductCommandHandler(outboxMessageFactory.Object, context, productService.Object);

            Func<Task> action = async () => await handler.Handle(new CreateProductCommand { Title = givenTitle }, CancellationToken.None);

            var exception = await action.Should().ThrowAsync<ProblemDetailsException>();
            exception.And.Value.Type.Should().Be("product-already-exist");
            exception.And.Value.Status.Should().Be(StatusCodes.Status409Conflict);
        }

        [Test]
        public async Task successfully_create_product_with_is_active_status_false()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<IOutboxMessageFactory> outboxMessageFactory = new Mock<IOutboxMessageFactory>();

            string givenTitle = "new test title";
            handler = new CreateProductCommandHandler(outboxMessageFactory.Object, context, productService.Object);

            var result = await handler.Handle(new CreateProductCommand { Title = givenTitle }, CancellationToken.None);

            result.Should().NotBeNull();

            var insertedItem = context.Products.First(x => x.Title.Equals(givenTitle));
            insertedItem.IsActive.Should().BeFalse();
            insertedItem.CategoryId.Should().BeNull();
        }

        [Test]
        public async Task successfully_create_product_with_category_and_is_active_status_false()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);
            Mock<IProductService> productService = new Mock<IProductService>();
            Mock<IOutboxMessageFactory> outboxMessageFactory = new Mock<IOutboxMessageFactory>();

            string givenTitle = "new test title";

            Category category = new Category(10, categoryName);
            context.Categories.Add(category);
            await context.SaveChangesAsync(CancellationToken.None);

            handler = new CreateProductCommandHandler(outboxMessageFactory.Object, context, productService.Object);

            var result = await handler.Handle(new CreateProductCommand { Title = givenTitle, CategoryId = category.Id }, CancellationToken.None);

            result.Should().NotBeNull();

            var insertedItem = context.Products.First(x => x.Title.Equals(givenTitle));
            insertedItem.IsActive.Should().BeFalse();
            insertedItem.CategoryId.Should().Be(category.Id);
        }


        [Test]
        public async Task successfully_create_product_with_is_active_status_true()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);
            Mock<IProductService> productService = new Mock<IProductService>();
            productService
                .Setup(x => x.DecideProductActivity(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            Mock<IOutboxMessageFactory> outboxMessageFactory = new Mock<IOutboxMessageFactory>();

            string givenTitle = "new test title";
            handler = new CreateProductCommandHandler(outboxMessageFactory.Object, context, productService.Object);

            var result = await handler.Handle(new CreateProductCommand { Title = givenTitle }, CancellationToken.None);

            result.Should().NotBeNull();

            var insertedItem = context.Products.First(x => x.Title.Equals(givenTitle));
            insertedItem.IsActive.Should().BeTrue();
        }
    }
}

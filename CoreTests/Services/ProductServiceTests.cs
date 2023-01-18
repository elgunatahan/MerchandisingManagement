using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistence;

namespace CoreTests.Services
{
    public class ProductServiceTests
    {
        [Test]
        public async Task it_should_throw_when_category_not_found()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                             .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                             .Options;

            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);
            
            IProductService productService = new ProductService(context);

            Func<Task> action = async () => await productService.DecideProductActivity(new Product(1, "", 10, "test title"), CancellationToken.None);

            var exception = await action.Should().ThrowAsync<ProblemDetailsException>();
            exception.And.Value.Type.Should().Be("category-not-found");

        }

        [Test]
        public async Task it_should_return_false_when_category_id_not_provided()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                             .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                             .Options;

            MerchandisingManagementContext merchandisingManagementContext = new MerchandisingManagementContext(options);

            IProductService productService = new ProductService(merchandisingManagementContext);

            bool resutl = await productService.DecideProductActivity(new Product(null, "", 10, "test title"), CancellationToken.None);

            resutl.Should().BeFalse();
        }

        [Test]
        public async Task it_should_return_false_when_product_stock_quantity_less_than_category_min_stock_quantity()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                             .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                             .Options;

            MerchandisingManagementContext merchandisingManagementContext = new MerchandisingManagementContext(options);

            int categoryMinStockQuantity = 500;
            int productStockQuantity = 50;

            Category category = new Category(categoryMinStockQuantity, "test category name");
            merchandisingManagementContext.Categories.Add(category);
            merchandisingManagementContext.SaveChanges();

            IProductService productService = new ProductService(merchandisingManagementContext);

            bool resutl = await productService.DecideProductActivity(new Product(category.Id, "", productStockQuantity, "test title"), CancellationToken.None);

            resutl.Should().BeFalse();
        }



        [Test]
        public async Task it_should_return_true_when_everything_is_okey()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                             .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                             .Options;

            MerchandisingManagementContext merchandisingManagementContext = new MerchandisingManagementContext(options);

            int categoryMinStockQuantity = 5;
            int productStockQuantity = 50;

            Category category = new Category(categoryMinStockQuantity, "test category name");

            merchandisingManagementContext.Categories.Add(category);
            merchandisingManagementContext.SaveChanges();

            IProductService productService = new ProductService(merchandisingManagementContext);

            bool resutl = await productService.DecideProductActivity(new Product(category.Id, "", productStockQuantity, "test title"), CancellationToken.None);

            resutl.Should().BeTrue();
        }
    }
}

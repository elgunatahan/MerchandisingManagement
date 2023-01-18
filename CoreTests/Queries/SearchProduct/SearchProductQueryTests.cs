using Application.Queries.SearchProduct;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistence;

namespace Application.UnitTests.Queries.SearchProduct
{
    public class SearchProductQueryTests
    {
        private SearchProductQueryHandler handler;
        private Task<SearchProductRepresentation> result;

        [Test]
        public void empty_list_must_be_returned()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);

            handler = new SearchProductQueryHandler(context);

            result = handler.Handle(new SearchProductQuery() { Page = 1, PageSize = 10}, CancellationToken.None);

            result.Result.TotalCount.Should().Be(0);
            result.Result.Page.Should().Be(1);
            result.Result.SearchProductVMs.Count().Should().Be(0);
        }

        [Test]
        public async Task returned_products_must_be_returned_correctly()
        {
            DbContextOptions<MerchandisingManagementContext> options = new DbContextOptionsBuilder<MerchandisingManagementContext>()
                                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                              .Options;


            IMerchandisingManagementContext context = new MerchandisingManagementContext(options);

            string categoryName = "test category name";
            Category category = new Category(10, categoryName);
            context.Categories.Add(category);

            await context.SaveChangesAsync(CancellationToken.None);

            string firstProductTitle = "test title";
            string firstProductDescription = "test description";
            int firstProductStock = 100;
            int? firstProductCategory = null;
            Product product = new Product(firstProductCategory, firstProductDescription, firstProductStock, firstProductTitle);
            context.Products.Add(product);

            string secondProductTitle = "test title 2";
            string secondProductDescription = "test description 2";
            int secondProductStock = 50;
            int? secondProductCategory = category.Id;
            Product secondProduct = new Product(secondProductCategory, secondProductDescription, secondProductStock, secondProductTitle);
            context.Products.Add(secondProduct);

            await context.SaveChangesAsync(CancellationToken.None);

            handler = new SearchProductQueryHandler(context);

            result = handler.Handle(new SearchProductQuery() { Page = 1, PageSize = 10 }, CancellationToken.None);

            result.Result.TotalCount.Should().Be(2);
            result.Result.Page.Should().Be(1);
            result.Result.SearchProductVMs.Count().Should().Be(2);

            SearchProductVM firstItem = result.Result.SearchProductVMs.First(x => x.Id == product.Id);
            firstItem.CategoryName.Should().BeNull();
            firstItem.CategoryId.Should().BeNull();
            firstItem.Description.Should().Be(firstProductDescription);
            firstItem.StockQuantity.Should().Be(firstProductStock);
            firstItem.Title.Should().Be(firstProductTitle);

            SearchProductVM secondItem = result.Result.SearchProductVMs.First(x => x.Id == secondProduct.Id);
            secondItem.CategoryName.Should().Be(category.Name);
            secondItem.CategoryId.Should().Be(category.Id);
            secondItem.Description.Should().Be(secondProductDescription);
            secondItem.StockQuantity.Should().Be(secondProductStock);
            secondItem.Title.Should().Be(secondProductTitle);
        }
    }
}

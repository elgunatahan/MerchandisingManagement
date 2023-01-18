using Application.Queries.SearchProduct;
using Domain.Enums;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace CoreTests.Validations
{
    public class SearchProductQueryValidatorTest
    {
        SearchProductQueryValidator _validationRules;
        public SearchProductQueryValidatorTest()
        {
            _validationRules = new SearchProductQueryValidator();
        }

        [Test]
        public void Validate_EmptyInput_ShouldHaveValidationError()
        {
            var query = new SearchProductQuery();

            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Page);
            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PageSize);
        }
        
        [TestCase(0, 0)]
        [TestCase(-1, -1)]
        [TestCase(-1, 102)]
        public void Validate_InvalidPageAndPageSize_ShouldHaveValidationError(int page, int pageSize)
        {
            var query = new SearchProductQuery()
            {
                Page = page,
                PageSize = pageSize
            };

            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Page);
            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.PageSize);
        }

        [Test]
        public void Validate_InvalidMinAndMaxStockQuantity_ShouldHaveValidationError()
        {
            var query = new SearchProductQuery()
            {
                MaxStockQuantity = -1,
                MinStockQuantity = -1,
                Page = 1,
                PageSize = 1
            };

            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.MaxStockQuantity);
            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.MinStockQuantity);
        }

        [Test]
        public void Validate_InvalidOrderAndOrderBy_ShouldHaveValidationError()
        {
            var query = new SearchProductQuery()
            {
                Order = "test",
                OrderBy = "test",
                Page = 1,
                PageSize = 1
            };

            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Order);
            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.OrderBy);
        }


        [TestCase]
        public void Validate_ValidInput_ShouldPassTheTest()
        {
            var query = new SearchProductQuery()
            {
                Order = OrderDirection.Ascending.ToString(),
                OrderBy = OrderBy.CreatedAt.ToString(),
                Page = 1,
                PageSize = 1,
                Keyword = "testkeyword",
                MaxStockQuantity = 1,
                MinStockQuantity = 0
            };

            _validationRules.TestValidate(query).IsValid.Should().BeTrue();
        }
    }
}

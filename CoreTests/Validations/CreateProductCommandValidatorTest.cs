using Application.Commands.CreateProduct;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace CoreTests.Validations
{
    public class CreateProductCommandValidatorTest
    {
        CreateProductCommandValidator _validationRules;
        public CreateProductCommandValidatorTest()
        {
            _validationRules = new CreateProductCommandValidator();
        }

        [Test]
        public void Validate_EmptyInput_ShouldHaveValidationError()
        {
            var command = new CreateProductCommand();

            _validationRules.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Title);
        }

        [TestCase(null, -1, 0)]
        [TestCase("", -1, -1)]
        public void Validate_InvalidStockQuantityAndTitle_ShouldHaveValidationError(string title, int stockQuantity, int categoryId)
        {
            var command = new CreateProductCommand()
            {
                Title = title,
                StockQuantity = stockQuantity,
                CategoryId = categoryId
            };

            _validationRules.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Title);
            _validationRules.TestValidate(command).ShouldHaveValidationErrorFor(x => x.StockQuantity);
        }


        [TestCase(null, null, 0)]
        [TestCase(null, null, 1)]
        [TestCase(null, "", 1)]
        [TestCase(null, "test description", 1)]
        [TestCase(1, "test description", 1)]
        public void Validate_ValidInput_ShouldPassTheTest(int? category, string description, int stockQuantity)
        {
            var command = new CreateProductCommand()
            {
                Title = "test product",
                CategoryId = category,
                Description = description,
                StockQuantity = stockQuantity
            };

            _validationRules.TestValidate(command).IsValid.Should().BeTrue();
        }
    }
}

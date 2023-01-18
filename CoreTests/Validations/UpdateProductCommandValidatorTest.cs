using Application.Commands.UpdateProduct;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace CoreTests.Validations
{
    public class UpdateProductCommandValidatorTest
    {
        UpdateProductCommandValidator _validationRules;
        public UpdateProductCommandValidatorTest()
        {
            _validationRules = new UpdateProductCommandValidator();
        }

        [Test]
        public void Validate_EmptyInput_ShouldHaveValidationError()
        {
            var command = new UpdateProductCommand();

            _validationRules.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Id);
        }

        [TestCase(-1, 0)]
        [TestCase(-1, -1)]
        public void Validate_InvalidStockQuantityAndCategory_ShouldHaveValidationError(int stockQuantity, int categoryId)
        {
            var command = new UpdateProductCommand()
            {
                Id = 1,
                StockQuantity = stockQuantity,
                CategoryId = categoryId
            };

            _validationRules.TestValidate(command).ShouldHaveValidationErrorFor(x => x.CategoryId);
            _validationRules.TestValidate(command).ShouldHaveValidationErrorFor(x => x.StockQuantity);
        }


        [TestCase(null, null, 0)]
        [TestCase(null, null, 1)]
        [TestCase(null, "", 1)]
        [TestCase(null, "test description", 1)]
        [TestCase(1, "test description", 1)]
        public void Validate_ValidInput_ShouldPassTheTest(int? category, string description, int stockQuantity)
        {
            var command = new UpdateProductCommand()
            {
                Id = 1,
                CategoryId = category,
                Description = description,
                StockQuantity = stockQuantity
            };

            _validationRules.TestValidate(command).IsValid.Should().BeTrue();
        }
    }
}

using Application.Queries.GetProductById;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace CoreTests.Validations
{
    public class GetProductByIdQueryValidatorTest
    {
        GetProductByIdQueryValidator _validationRules;
        public GetProductByIdQueryValidatorTest()
        {
            _validationRules = new GetProductByIdQueryValidator();
        }

        [Test]
        public void Validate_InvalidInput_ShouldHaveValidationError()
        {
            var query = new GetProductByIdQuery();

            _validationRules.TestValidate(query).ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Test]
        public void Validate_ValidInput_ShouldPassTheTest()
        {
            var query = new GetProductByIdQuery()
            {
                Id = 1
            };

            _validationRules.TestValidate(query).ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}

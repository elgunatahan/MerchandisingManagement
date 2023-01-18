using FluentValidation;

namespace Application.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.MinStockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Name).MaximumLength(200).NotEmpty();
        }
    }
}

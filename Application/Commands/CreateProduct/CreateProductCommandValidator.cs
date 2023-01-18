using FluentValidation;

namespace Application.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Title).MaximumLength(200).NotEmpty();
            RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
        }
    }
}

using Domain.Enums;
using FluentValidation;

namespace Application.Queries.SearchProduct
{
    public class SearchProductQueryValidator : AbstractValidator<SearchProductQuery>
    {
        public SearchProductQueryValidator()
        {
            RuleFor(x => x.MaxStockQuantity).GreaterThanOrEqualTo(0).When(x => x.MaxStockQuantity.HasValue);
            RuleFor(x => x.MinStockQuantity).GreaterThanOrEqualTo(0).When(x => x.MinStockQuantity.HasValue);
            
            RuleFor(x => x.Page).GreaterThan(0);

            RuleFor(x => x.PageSize).GreaterThan(0);
            RuleFor(x => x.PageSize).LessThan(101);

            RuleFor(x => x.OrderBy).Must(x => Enum.IsDefined(typeof(OrderBy), x))
                .WithMessage("OrderBy type is not valid")
                .When(x => !string.IsNullOrWhiteSpace(x.OrderBy));

            RuleFor(x => x.Order).Must(x => Enum.IsDefined(typeof(OrderDirection), x))
                .WithMessage("Order direction type is not valid")
                .When(x => !string.IsNullOrWhiteSpace(x.Order));
        }
    }
}

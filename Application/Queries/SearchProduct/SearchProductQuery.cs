using MediatR;

namespace Application.Queries.SearchProduct
{
    public class SearchProductQuery : IRequest<SearchProductRepresentation>
    {
        public string Keyword { get; set; }

        public int? MaxStockQuantity { get; set; }

        public int? MinStockQuantity { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string OrderBy { get; set; }

        public string Order { get; set; }
    }
}

using Application.Queries.SearchProduct;

namespace Api.V1.Dtos
{
    public class SearchProductInput
    {
        /// <summary>
        /// Requested keyword for query
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Requested maximum stock quantity for query
        /// </summary>
        public int? MaxStockQuantity { get; set; }

        /// <summary>
        /// Requested minimum stock quantity for query
        /// </summary>
        public int? MinStockQuantity { get; set; }

        /// <summary>
        /// Requested page no
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Requested page item count
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Requested order by definition
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Requested order direction
        /// </summary>
        public string Order { get; set; }

        internal SearchProductQuery ToQuery()
        {
            return new SearchProductQuery
            {
                Keyword = Keyword,
                MaxStockQuantity = MaxStockQuantity,
                MinStockQuantity = MinStockQuantity,
                Order = Order,
                OrderBy = OrderBy,
                Page = Page,
                PageSize = PageSize
            };
        }
    }
}

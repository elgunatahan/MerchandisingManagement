namespace Application.Queries.SearchProduct
{
    public class SearchProductRepresentation
    {
        public List<SearchProductVM> SearchProductVMs { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
    }

    public class SearchProductVM
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int StockQuantity { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

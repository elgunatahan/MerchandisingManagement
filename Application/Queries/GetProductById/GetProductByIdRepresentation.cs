namespace Application.Queries.GetProductById
{
    public class GetProductByIdRepresentation
    {
        public long Id { get; set; }

        public int? CategoryId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int StockQuantity { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

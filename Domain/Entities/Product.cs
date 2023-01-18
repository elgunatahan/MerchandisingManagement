namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public virtual Category Category { get; private set; }

        public int? CategoryId { get; private set; }

        public string Description { get; private set; }

        public bool IsActive { get; private set; }

        public int StockQuantity { get; private set; }

        public string Title { get; private set; }

        public DateTime CreatedAt { get; private set; }
        
        public string CreatedBy { get; private set; }

        public DateTime? UpdatedAt { get; private set; }
        
        public string UpdatedBy { get; private set; }

        public Product(int? categoryId, string description, int stockQuantity, string title)
        {
            CategoryId = categoryId;
            Description = description;
            IsActive = CategoryId.HasValue;
            StockQuantity = stockQuantity;
            Title = title;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = "MerchandisingManagementApi";
        }

        public void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void Update(int? categoryId, int stockQuantity,  string description)
        {
            CategoryId = categoryId;
            Description = description;
            StockQuantity = stockQuantity;
        }
    }
}

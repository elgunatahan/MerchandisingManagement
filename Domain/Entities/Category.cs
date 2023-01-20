using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public int MinStockQuantity { get; private set; }

        public string Name { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public string UpdatedBy { get; private set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Category(int minStockQuantity, string name)
        {
            MinStockQuantity = minStockQuantity;
            Name = name;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = "MerchandisingManagementApi";
        }
    }
}

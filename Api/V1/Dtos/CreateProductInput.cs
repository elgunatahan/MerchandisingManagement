using Application.Commands.CreateProduct;

namespace Api.V1.Dtos
{
    public class CreateProductInput
    {
        public int? CategoryId { get; set; }

        public string Description { get; set; }
        
        public int StockQuantity { get; set; }
        
        public string Title { get; set; }

        internal CreateProductCommand ToCommand()
        {
            return new CreateProductCommand
            {
                CategoryId = CategoryId,
                Description = Description,
                StockQuantity = StockQuantity,
                Title = Title
            };
        }
    }
}

using Application.Commands.UpdateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Dtos
{
    public class UpdateProductInput
    {
        [FromRoute(Name = "id")]
        public long Id { get; set; }

        public int? CategoryId { get; set; }

        public string Description { get; set; }
        
        public int StockQuantity { get; set; }

        internal UpdateProductCommand ToCommand()
        {
            return new UpdateProductCommand
            {
                Id = Id,
                CategoryId = CategoryId,
                Description = Description,
                StockQuantity = StockQuantity,
            };
        }
    }
}

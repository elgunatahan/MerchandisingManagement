using MediatR;

namespace Application.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public long Id { get; set; }

        public int? CategoryId { get; set; }

        public string Description { get; set; }
        
        public int StockQuantity { get; set; }
    }
}

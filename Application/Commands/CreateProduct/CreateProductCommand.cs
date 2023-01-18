using MediatR;

namespace Application.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<CreateProductRepresentation>
    {
        public int? CategoryId { get; set; }

        public string Description { get; set; }
        
        public int StockQuantity { get; set; }
        
        public string Title { get; set; }
    }
}

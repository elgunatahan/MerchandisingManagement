using MediatR;

namespace Application.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<CreateCategoryRepresentation>
    {
        public int MinStockQuantity { get; set; }
        
        public string Name { get; set; }
    }
}

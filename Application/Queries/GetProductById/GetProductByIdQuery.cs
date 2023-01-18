using MediatR;

namespace Application.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<GetProductByIdRepresentation>
    {
        public int Id { get; set; }
    }
}

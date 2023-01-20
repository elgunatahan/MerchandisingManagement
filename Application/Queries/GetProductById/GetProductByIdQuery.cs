using MediatR;

namespace Application.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<GetProductByIdRepresentation>
    {
        public long Id { get; set; }
    }
}

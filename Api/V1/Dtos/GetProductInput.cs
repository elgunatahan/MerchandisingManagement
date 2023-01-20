using Application.Queries.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Dtos
{
    public class GetProductInput
    {
        [FromRoute(Name = "id")]
        public long Id { get; set; }

        internal GetProductByIdQuery ToQuery()
        {
            return new GetProductByIdQuery
            {
                Id = Id,
            };
        }
    }
}

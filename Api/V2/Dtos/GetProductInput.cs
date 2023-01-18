using Microsoft.AspNetCore.Mvc;

namespace Api.V2.Dtos
{
    public class GetProductInput
    {
        [FromRoute(Name = "id")]
        public long Id { get; set; }
    }
}

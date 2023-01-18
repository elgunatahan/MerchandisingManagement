using Api.V2.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.V2.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/products")]
    [ApiExplorerSettings(GroupName = "Products")]
    public class ProductControllerV2 : ControllerBase
    {

        [HttpGet("{id}")]
        public IActionResult GetProduct([FromQuery] GetProductInput input)
        {
            return Ok();
        }
    }
}

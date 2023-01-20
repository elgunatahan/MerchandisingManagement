using Api.V1.Dtos;
using Application.Commands.CreateProduct;
using Application.Queries.GetProductById;
using Application.Queries.SearchProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/products")]
    [ApiExplorerSettings(GroupName = "Products")]
    public class ProductControllerV1 : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductControllerV1(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create product with given input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductRepresentation))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateProduct([FromQuery] CreateProductInput input, CancellationToken cancellationToken)
        {
            try
            {

                CreateProductRepresentation response = await _mediator.Send(input.ToCommand(), cancellationToken);
                return Created($"v1/products/{response.Id}", response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get product by given id
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductByIdRepresentation))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetProduct([FromQuery] GetProductInput input, CancellationToken cancellationToken)
        {
            GetProductByIdRepresentation response = await _mediator.Send(input.ToQuery(), cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Update product by given id with input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateProduct([FromQuery] UpdateProductInput input, CancellationToken cancellationToken)
        {
            await _mediator.Send(input.ToCommand(), cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Search product with given input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchProductRepresentation))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductInput input, CancellationToken cancellationToken)
        {
            SearchProductRepresentation response = await _mediator.Send(input.ToQuery(), cancellationToken);
            return Ok(response);
        }
    }
}

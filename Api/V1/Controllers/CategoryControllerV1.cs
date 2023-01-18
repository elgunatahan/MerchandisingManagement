using Api.V1.Dtos;
using Application.Commands.CreateCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/categories")]
    [ApiExplorerSettings(GroupName = "Categories")]
    public class CategoryControllerV1 : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryControllerV1(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create category
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCategoryRepresentation))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateCategory([FromQuery] CreateCategoryInput input, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(input.ToCommand(), cancellationToken);
            return Created($"v1/categories/{response.Id}", response);
        }
    }
}

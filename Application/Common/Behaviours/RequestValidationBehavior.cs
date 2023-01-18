using Domain.Common.Filters;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Behaviours
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            var errors = new Dictionary<string, string[]>();
            foreach (var failure in failures)
            {
                if (errors.ContainsKey(failure.PropertyName))
                {
                    errors[failure.PropertyName] = errors[failure.PropertyName].Append(failure.ErrorMessage).ToArray();
                }
                else
                {
                    errors.Add(failure.PropertyName, new string[] { failure.ErrorMessage });
                }
            }

            if (failures.Count != 0)
            {
                ValidationProblemDetails validationProblemDetails = new ValidationProblemDetails(errors);
                validationProblemDetails.Status = StatusCodes.Status400BadRequest;
                validationProblemDetails.Type = "validation-error";
                validationProblemDetails.Title = "Validation Failed";
                validationProblemDetails.Detail = "One or more inputs need to be corrected.";
                validationProblemDetails.Instance = Guid.NewGuid().ToString();

                throw new ProblemDetailsException(validationProblemDetails);
            }

            return next();
        }
    }
}

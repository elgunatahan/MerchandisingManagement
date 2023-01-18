using Microsoft.AspNetCore.Mvc;

namespace Domain.Common.Filters
{
    public class ProblemDetailsException : Exception
    {
        public ProblemDetailsException(ProblemDetails value)
        {
            Value = value;
        }

        public ProblemDetails Value { get; }
    }
}

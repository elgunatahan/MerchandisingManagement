using Domain.Common.Filters;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryRepresentation>
    {
        private readonly IMerchandisingManagementContext _merchandisingManagementContext;

        public CreateCategoryCommandHandler(IMerchandisingManagementContext merchandisingManagementContext)
        {
            _merchandisingManagementContext = merchandisingManagementContext;
        }
        public async Task<CreateCategoryRepresentation> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            bool isCategoryAlreadyExist = await _merchandisingManagementContext.Categories.AnyAsync(x => x.Name.Equals(request.Name), cancellationToken);

            if (isCategoryAlreadyExist)
            {
                ProblemDetails problemDetails = new ProblemDetails()
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Category has already exist.",
                    Type = "category-already-exist",
                    Detail = "There is already category record for the name"
                };

                throw new ProblemDetailsException(problemDetails);
            }

            Category category = new Category(request.MinStockQuantity, request.Name);

            _merchandisingManagementContext.Categories.Add(category);

            await _merchandisingManagementContext.SaveChangesAsync(cancellationToken);

            return new CreateCategoryRepresentation { Id = category.Id };
        }
    }
}

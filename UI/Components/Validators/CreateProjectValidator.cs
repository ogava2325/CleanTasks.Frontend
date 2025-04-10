using System.Data;
using Domain.Dtos.Project;
using FluentValidation;

namespace UI.Components.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");
    }
}
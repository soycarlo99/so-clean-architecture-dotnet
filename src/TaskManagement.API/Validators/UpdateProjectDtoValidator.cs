using FluentValidation;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Validators;

public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectDtoValidator()
    {
        // All fields optional for PATCH, but validate if provided
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
            .When(x => x.Name != null);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty when provided")
            .When(x => x.Name != null);
    }
}

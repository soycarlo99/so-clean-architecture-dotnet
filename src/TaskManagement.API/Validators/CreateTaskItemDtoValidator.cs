using FluentValidation;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Validators;

public class CreateTaskItemDtoValidator : AbstractValidator<CreateTaskItemDto>
{
    public CreateTaskItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.ProjectId)
            .GreaterThan(0)
            .WithMessage("ProjectId must be a valid positive number");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0)
            .When(x => x.EstimatedHours.HasValue)
            .WithMessage("EstimatedHours must be positive when provided");
    }
}

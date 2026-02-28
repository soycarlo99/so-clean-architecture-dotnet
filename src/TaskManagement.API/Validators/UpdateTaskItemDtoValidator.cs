using FluentValidation;
using TaskManagement.API.DTOs;
using TaskManagement.Core.Enums;

namespace TaskManagement.API.Validators;

public class UpdateTaskItemDtoValidator : AbstractValidator<UpdateTaskItemDto>
{
    public UpdateTaskItemDtoValidator()
    {
        // All fields optional for PATCH, but validate if provided
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .When(x => x.Title != null);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title cannot be empty when provided")
            .When(x => x.Title != null);

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("EstimatedHours must be positive when provided")
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.AssignedToUserId)
            .GreaterThan(0).WithMessage("AssignedToUserId must be a valid positive number")
            .When(x => x.AssignedToUserId.HasValue);

        RuleFor(x => x.Status)
            .Must(BeValidStatus).WithMessage("Status must be one of: Pending, InProgress, InReview, Done")
            .When(x => x.Status != null);
    }

    private bool BeValidStatus(string? status)
    {
        return Enum.TryParse<TaskItemStatus>(status, ignoreCase: true, out _);
    }
}

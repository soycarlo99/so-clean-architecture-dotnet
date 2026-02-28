using FluentValidation;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Validators;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        // All fields optional for PATCH, but validate if provided
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters")
            .When(x => x.Email != null);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty when provided")
            .When(x => x.Email != null);

        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
            .When(x => x.FullName != null);

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name cannot be empty when provided")
            .When(x => x.FullName != null);
    }
}

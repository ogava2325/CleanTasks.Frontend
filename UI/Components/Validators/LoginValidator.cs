using System.Data;
using Domain.Dtos.User;
using FluentValidation;

namespace UI.Components.Validators;

public class LoginValidator : AbstractValidator<LoginUserDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
    
}
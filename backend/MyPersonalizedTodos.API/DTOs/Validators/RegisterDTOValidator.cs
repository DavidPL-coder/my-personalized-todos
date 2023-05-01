using FluentValidation;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Enums;
using MyPersonalizedTodos.API.Extensions;

namespace MyPersonalizedTodos.API.DTOs.Validators;

public class RegisterDTOValidator : AbstractValidator<RegisterUserDTO>
{
    //TODO: Improve 'Age' and 'DateOfBirth' validation

    public RegisterDTOValidator(AppDbContext dbContext)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(dto => dto.Login)
            .NotEmpty()
            .MinimumLength(4)
            .Must(property => !property.Any(char.IsWhiteSpace)).WithMessage("'{PropertyName}' can't contain whitespaces.")
            .Must(providedLogin => dbContext.Users.All(user => user.Name != providedLogin)).WithMessage("Provided '{PropertyName}' is reserved by other user.");

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Must(property => !property.Any(char.IsWhiteSpace)).WithMessage("'{PropertyName}' can't contain whitespaces.");

        RuleFor(dto => dto.ConfirmPassword)
            .Equal(dto => dto.Password).WithMessage("'{PropertyName}' has to match to {ComparisonProperty}.");

        RuleFor(dto => dto.Age)
            .NotEmpty()
            .InclusiveBetween(5, 200);

        RuleFor(dto => dto.DateOfBirth)
            .NotEmpty();

        RuleFor(dto => dto.Gender)
            .NotEmpty()
            .Must(property => property.IsConvertiableToEnum<UserGender>()).WithMessage("The given gender doesn't exist.");

        RuleFor(dto => dto.Purposes)
            .NotEmpty()
            .Must(purposes => purposes.Count <= 3).WithMessage("No more than 3 purposes are allowed")
            .ForEach(purposeRule =>
                purposeRule.Must(purpose => purpose.IsConvertiableToEnum<Purpose>()).WithMessage("'{PropertyValue}' is not valid purpose.")
            );

        RuleFor(dto => dto.Nationality)
            .NotEmpty()
            .Must(property => property.IsConvertiableToEnum<UserNationality>()).WithMessage("The given nationality doesn't exist in the system.");
    }
}

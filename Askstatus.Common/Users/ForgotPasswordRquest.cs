using FluentValidation;

namespace Askstatus.Common.Users;
public sealed class ForgotPasswordRquest
{
    public ForgotPasswordRquest()
    {
        Email = string.Empty;
    }

    public ForgotPasswordRquest(string email)
    {
        Email = email;
    }

    public string Email { get; set; }
}   


public class ForgotPasswordRquestFluentValidator : AbstractValidator<ForgotPasswordRquest>
{
    public ForgotPasswordRquestFluentValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ForgotPasswordRquest>.CreateWithOptions((ForgotPasswordRquest)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}

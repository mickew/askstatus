using FluentValidation;

namespace Askstatus.Common.Users;
public sealed class ResetPasswordRequest
{
    public ResetPasswordRequest()
    {
        UserId = string.Empty;
        Token = string.Empty;
        NewPassword = string.Empty;
    }

    public ResetPasswordRequest(string userId, string token, string newPassword)
    {
        UserId = userId;
        Token = token;
        NewPassword = newPassword;
    }

    public string? UserId { get; set; }
    public string? Token { get; set; }
    public string? NewPassword { get; set; }

}

public class ResetPasswordRequestFluentValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestFluentValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        RuleFor(x => x.Token)
            .NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ResetPasswordRequest>.CreateWithOptions((ResetPasswordRequest)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}

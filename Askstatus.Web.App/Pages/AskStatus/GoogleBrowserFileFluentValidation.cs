using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace Askstatus.Web.App.Pages.AskStatus;

public class GoogleBrowserFileFluentValidation : AbstractValidator<IBrowserFile>
{
    public GoogleBrowserFileFluentValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("File name is required");
        RuleFor(x => x.Name).Matches(@"^Google.Apis.Auth.OAuth2.Responses.TokenResponse-(.+)@gmail.com$").WithMessage("File is not a google token response file");
        RuleFor(x => x.Size).LessThanOrEqualTo(1024 * 1024).WithMessage("File size must be less than 1MB");
        RuleFor(x => x.ContentType).Must(x => x == "application/octet-stream").WithMessage("File must be of type application/octet-stream");
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace Askstatus.Web.App.Pages.AskStatus;

public class ProductionAppsettingsFileFluentValidation : AbstractValidator<IBrowserFile>
{
    public ProductionAppsettingsFileFluentValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("File name is required");
        RuleFor(x => x.Name).Matches(@"^appsettings.Production.json$").WithMessage("File is not a production appsettings file");
        RuleFor(x => x.Size).LessThanOrEqualTo(1024 * 1024).WithMessage("File size must be less than 1MB");
        RuleFor(x => x.ContentType).Must(x => x == "application/json").WithMessage("File must be of type application/json");
    }
}

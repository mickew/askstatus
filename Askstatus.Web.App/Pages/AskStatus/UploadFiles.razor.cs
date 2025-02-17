using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Refit;

namespace Askstatus.Web.App.Pages.AskStatus;

public partial class UploadFiles
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ILogger<UploadFiles> Logger { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    private async Task UploadGoogleFile(IBrowserFile file)
    {
        var validationResult = await new GoogleBrowserFileFluentValidation().ValidateAsync(file);
        if (!validationResult.IsValid)
        {
            Snackbar.Add(validationResult.Errors.First().ErrorMessage, Severity.Error);
            return;
        }
        var result = await ApiService.SystemAPI.UploadGoogleTokenResponseFile(new StreamPart(file.OpenReadStream(), file.Name, file.ContentType, "file"));
        if (!result.IsSuccessStatusCode)
        {
            Logger.LogError(result.Error, result.Error.Content);
            Snackbar.Add(result.Error.Content!, Severity.Error);
            return;
        }
        Snackbar.Add("File uploaded successfully", Severity.Success);
    }

    private async Task UploadAppSettingsFile(IBrowserFile file)
    {
        var validationResult = await new ProductionAppsettingsFileFluentValidation().ValidateAsync(file);
        if (!validationResult.IsValid)
        {
            Snackbar.Add(validationResult.Errors.First().ErrorMessage, Severity.Error);
            return;
        }
        var result = await ApiService.SystemAPI.UploadProductionAppSettingsFile(new StreamPart(file.OpenReadStream(), file.Name, file.ContentType, "file"));
        if (!result.IsSuccessStatusCode)
        {
            Logger.LogError(result.Error, result.Error.Content);
            Snackbar.Add(result.Error.Content!, Severity.Error);
            return;
        }
        Snackbar.Add("File uploaded successfully", Severity.Success);
    }
}

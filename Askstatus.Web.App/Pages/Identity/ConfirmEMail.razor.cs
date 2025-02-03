using Askstatus.Common.Users;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Askstatus.Web.App.Pages.Identity;

public partial class ConfirmEMail
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ILogger<ConfirmEMail> Logger { get; set; } = null!;

    [Inject]
    public NavigationManager? Navigation { get; set; } = null!;

    public string? UserId { get; set; }

    public string? Token { get; set; }

    private bool IsConfirmed { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("OnInitializedAsync started");
        await GetQueryStringValues();
        Logger.LogInformation("Query string userid : {0}", UserId);
        Logger.LogInformation("Query string token : {0}", Token);
        if (Token != null && UserId != null)
        {
            await ConfirmEmail();
        }
    }

    private Task GetQueryStringValues()
    {
        var uri = new Uri(Navigation!.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);
        try
        {
            UserId = query["userId"];
            Token = query["token"];
        }
        catch (Exception)
        {
            Logger.LogError("Error getting query string values userId or token");
        }
        return Task.CompletedTask;
    }

    private async Task ConfirmEmail()
    {
        var response = await ApiService.UserAPI.ConfirmEmail(new ConfirmEmailRequest(UserId!, Token!));
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            IsConfirmed = false;
        }
        else
        {
            IsConfirmed = true;
        }
    }
}

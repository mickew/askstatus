using Askstatus.Web.App.Identity;
using Microsoft.AspNetCore.Components;

namespace Askstatus.Web.App.Pages.Identity;

public partial class Logout
{
    [Inject] IAccountManagement? Acct { get; set; }

    [Inject] NavigationManager? NavigationManager { get; set; }

[Parameter]
    public string returnUrl { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (await Acct!.CheckAuthenticatedAsync())
        {
            await Acct.LogoutAsync();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                NavigationManager!.NavigateTo(returnUrl);
            }
        }
        NavigationManager!.NavigateTo("/");

        await base.OnInitializedAsync();
    }
}

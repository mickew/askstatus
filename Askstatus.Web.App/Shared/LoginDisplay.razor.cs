using System.Security.Claims;
using Askstatus.Domain.Constants;
//using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Askstatus.Web.App.Shared;

public partial class LoginDisplay
{
    [Inject]
    public NavigationManager? Navigation { get; set; } = null!;

    //[Inject]
    //public AskstatusApiService? ApiService { get; set; } = null!;

    private void BeginLogOut()
    {
        //ApiService!.IdentityApi.Logout();
        Navigation!.NavigateToLogout("logout", "/");
    }

    private string GetFullName(ClaimsPrincipal principal)
    {
        var firstName = principal.Claims.FirstOrDefault(
            c => c.Type == CustomClaimTypes.FirstName);
        var lastName = principal.Claims.FirstOrDefault(
            c => c.Type == CustomClaimTypes.LastName);
        if (firstName != null && lastName != null)
        {
            return $"{firstName.Value} {lastName.Value}";
        }
        return "?";
    }
}

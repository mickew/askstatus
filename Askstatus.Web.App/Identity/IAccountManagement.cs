using Askstatus.Web.App.Identity.Models;

namespace Askstatus.Web.App.Identity;

public interface IAccountManagement
{
    public Task<FormResult> LoginAsync(string userName, string password);

    public Task LogoutAsync();

    public Task<bool> CheckAuthenticatedAsync();
}

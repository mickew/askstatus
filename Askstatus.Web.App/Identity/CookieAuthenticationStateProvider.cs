using System.Security.Claims;
using Askstatus.Common.Identity;
using Askstatus.Domain.Constants;
using Askstatus.Sdk;
using Askstatus.Web.App.Identity.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Askstatus.Web.App.Identity;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider, IAccountManagement
{
    private readonly AskstatusApiService _askstatusApi;
    private readonly ILogger<CookieAuthenticationStateProvider> _logger;

    private bool _authenticated = false;

    private readonly ClaimsPrincipal Unauthenticated =
        new(new ClaimsIdentity());

    public CookieAuthenticationStateProvider(ILogger<CookieAuthenticationStateProvider> logger, AskstatusApiService askstatusApi)
    {
        _logger = logger;
        _askstatusApi = askstatusApi;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;

        // default to not authenticated
        var user = Unauthenticated;

        try
        {
            // the user info endpoint is secured, so if the user isn't logged in this will fail
            var userResponse = await _askstatusApi.IdentityApi.GetUserInfo();

            // throw if user info wasn't retrieved
            if (userResponse.IsSuccessStatusCode && userResponse.Content is not null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, userResponse.Content.UserName),
                    new(ClaimTypes.Email, userResponse.Content.Email),
                    new(CustomClaimTypes.FirstName, userResponse.Content.FirstName),
                    new(CustomClaimTypes.LastName, userResponse.Content.LastName),
                };

                var applicationResponse = await _askstatusApi.IdentityApi.GetApplicationClaims();
                if (applicationResponse.IsSuccessStatusCode && applicationResponse.Content is not null)
                {
                    foreach (var claim in applicationResponse.Content)
                    {
                        claims.Add(new Claim(claim.Type, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer));
                    }
                }
                var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                user = new ClaimsPrincipal(id);
                _authenticated = true;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when getting AuthenticationState");
        }

        // return the state
        return new AuthenticationState(user);
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _authenticated;
    }

    public async Task<FormResult> LoginAsync(string userName, string password)
    {
        try
        {
            var res = await _askstatusApi.IdentityApi.Login(new LoginRequest(userName, password));
            if (res.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new FormResult { Succeeded = true };
            }
            else
            {
                return new FormResult { Succeeded = false, ErrorList = new[] { res.Error.Message } };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in");
            return new FormResult { Succeeded = false, ErrorList = new[] { "Invalid login attempt, check log for information" } };
            //throw;
        }
    }

    public async Task LogoutAsync()
    {
        await _askstatusApi.IdentityApi.Logout();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

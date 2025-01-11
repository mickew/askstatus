using System.Security.Claims;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Identity;
using Askstatus.Domain.Constants;
using Askstatus.Infrastructure.Identity;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Askstatus.Infrastructure.Services;
public sealed class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(SignInManager<ApplicationUser> signInManager, ILogger<IdentityService> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public Task<Result<IEnumerable<ApplicationClaimVM>>> GetApplicationClaims()
    {
        var claimsPrincipal = _signInManager.Context.User;
        if (claimsPrincipal.Identity is not null && claimsPrincipal.Identity.IsAuthenticated)
        {
            var identity = (ClaimsIdentity)claimsPrincipal.Identity;

            var claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == CustomClaimTypes.Permissions)
                .Select(c =>
                    new ApplicationClaimVM(c.Issuer, c.OriginalIssuer, c.Type, c.Value, c.ValueType));
            _logger.LogInformation("Claims {Claims} found", claims);
            return Task.FromResult(Result.Ok(claims));
        }
        _logger.LogWarning("Not authorized");
        return Task.FromResult(Result.Fail<IEnumerable<ApplicationClaimVM>>(new IdentityUnauthorizedError("Not authorized")));
    }

    public async Task<Result<UserInfoVM>> GetUserInfo()
    {
        var claimsPrincipal = _signInManager.Context.User;
        var user = await _signInManager.UserManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is not null && user is ApplicationUser)
        {
            var userInfo = new UserInfoVM(user.Id, user.UserName!, user.FirstName!, user.LastName!, user.Email!);
            _logger.LogInformation("User with info {UserInfo} found", userInfo);
            return Result.Ok(userInfo);
        }
        _logger.LogWarning("User not found");
        return Result.Fail<UserInfoVM>(new IdentityNotFoundError("User not found"));
    }

    public async Task<Result> Login(LoginRequest loginRequest)
    {
        _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;
        var result = await _signInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, true, lockoutOnFailure: true).ConfigureAwait(false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User {User} logged in", loginRequest.UserName);
            return Result.Ok();
        }
        _logger.LogWarning("Login failed for user {User}", loginRequest.UserName);
        return Result.Fail(new IdentityUnauthorizedError("Login failed"));
    }

    public async Task<Result> Logout()
    {
        await _signInManager.SignOutAsync().ConfigureAwait(false);
        _logger.LogInformation("User logged out");
        return Result.Ok();
    }
}

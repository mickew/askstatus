using System.Security.Claims;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Models.Identity;
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

    public Task<Result<IEnumerable<ApplicationClaimDto>>> GetApplicationClaims()
    {
        var claimsPrincipal = _signInManager.Context.User;
        if (claimsPrincipal.Identity is not null && claimsPrincipal.Identity.IsAuthenticated)
        {
            var identity = (ClaimsIdentity)claimsPrincipal.Identity;

            var claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == CustomClaimTypes.Permissions)
                .Select(c =>
                    new ApplicationClaimDto(c.Issuer, c.OriginalIssuer, c.Type, c.Value, c.ValueType));
            return Task.FromResult(Result.Ok(claims));
        }
        _logger.LogWarning("Not authorized");
        return Task.FromResult(Result.Fail<IEnumerable<ApplicationClaimDto>>("Not authorized"));
    }

    public async Task<Result<UserInfoDto>> GetUserInfo()
    {
        var claimsPrincipal = _signInManager.Context.User;
        var user = await _signInManager.UserManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is not null && user is ApplicationUser)
        {
            return Result.Ok(new UserInfoDto(user.Id, user.UserName!, user.Email!));
        }
        _logger.LogWarning("User not found");
        return Result.Fail<UserInfoDto>("User not found");
    }

    public async Task<Result> Login(LoginDto loginRequest)
    {
        _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;
        var result = await _signInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, true, lockoutOnFailure: true).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning($"Login failed for user {loginRequest.UserName}");
        return Result.Fail("Login failed");
    }

    public async Task<Result> Logout()
    {
        await _signInManager.SignOutAsync().ConfigureAwait(false);
        return Result.Ok();
    }
}

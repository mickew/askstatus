using Askstatus.Application.Models.Identity;
using FluentResults;

namespace Askstatus.Application.Interfaces;
public interface IIdentityService
{
    Task<Result> Login(LoginDto loginRequest);

    Task<Result> Logout();

    Task<Result<UserInfoDto>> GetUserInfo();

    Task<Result<IEnumerable<ApplicationClaimDto>>> GetApplicationClaims();
}

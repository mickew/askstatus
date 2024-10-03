using Askstatus.Common.Identity;
using FluentResults;

namespace Askstatus.Application.Interfaces;
public interface IIdentityService
{
    Task<Result> Login(LoginRequest loginRequest);

    Task<Result> Logout();

    Task<Result<UserInfoVM>> GetUserInfo();

    Task<Result<IEnumerable<ApplicationClaimVM>>> GetApplicationClaims();
}

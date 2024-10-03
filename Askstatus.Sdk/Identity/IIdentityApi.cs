using System;
using System.Collections.Generic;
using System.Text;
using Askstatus.Common.Identity;
using Refit;

namespace Askstatus.Sdk.Identity;
public interface IIdentityApi
{
    [Post("/api/identity/login")]
    Task<IApiResponse> Login([Body] LoginRequest login);

    [Post("/api/identity/logout")]
    Task<IApiResponse> Logout();

    [Get("/api/identity/userinfo")]
    Task<IApiResponse<UserInfoVM>> GetUserInfo();

    [Get("/api/identity/claims")]
    Task<IApiResponse<IEnumerable<ApplicationClaimVM>>> GetApplicationClaims();
}

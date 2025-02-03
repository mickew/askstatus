using Askstatus.Common.Users;
using Refit;

namespace Askstatus.Sdk.Users;
public interface IUserAPI
{
    [Get("/api/user")]
    Task<IApiResponse<IEnumerable<UserVM>>> GetUsers();

    [Get("/api/user/{id}")]
    Task<IApiResponse<UserVM>> GetUserById(string id);

    [Post("/api/user")]
    Task<IApiResponse<UserVM>> CreateUser(UserRequest request);

    [Put("/api/user")]
    Task<IApiResponse> UpdateUser(UserRequest request);

    [Delete("/api/user/{id}")]
    Task<IApiResponse> DeleteUser(string id);

    [Put("/api/user/{id}/reset-password")]
    Task<IApiResponse> ResetPassword(string id);

    [Put("/api/user/change-password")]
    Task<IApiResponse> ChangePassword(ChangePasswordRequest request);

    [Post("/api/user/confirm-email")]
    Task<IApiResponse> ConfirmEmail(ConfirmEmailRequest request);

    [Post("/api/user/forgot-password")]
    Task<IApiResponse> ForgotPassword(ForgotPasswordRquest request);

    [Post("/api/user/reset-password")]
    Task<IApiResponse> ResetUserPassword(ResetPasswordRequest request);
}

using Askstatus.Application.Users;
using Askstatus.Common.Users;
using FluentResults;

namespace Askstatus.Application.Interfaces;
public interface IUserService
{
    Task<Result<IEnumerable<UserVM>>> GetUsers();

    Task<Result<UserVM>> GetUserById(string Id);

    Task<Result> UpdateUser(UserRequest userRequest);

    Task<Result<UserVMWithLink>> CreateUser(UserRequest userRequest);

    Task<Result> DeleteUser(string Id);

    Task<Result> ResetPassword(string Id);

    Task<Result> ChangePassword(ChangePasswordRequest changePasswordRequest);

    Task<Result<AccessControlVm>> GetAccessControlConfiguration();

    Task<Result> UpdateAccessControlConfiguration(RoleRequest roleRequest);

    Task<Result<IEnumerable<RoleDto>>> GetRoles();

    Task<Result<RoleDto>> CreateRole(RoleRequest roleRequest);

    Task<Result> UpdateRole(RoleRequest roleRequest);

    Task<Result> DeleteRole(string Id);

    Task<Result> ConfirmEmail(string Id, string code);

    Task<Result<UserVMWithLink>> ForgotPassword(string email);

    Task<Result> ResetUserPassword(string Id, string Token, string NewPassword);
}

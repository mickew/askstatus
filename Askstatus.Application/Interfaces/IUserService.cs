using Askstatus.Common.Users;
using FluentResults;

namespace Askstatus.Application.Interfaces;
public interface IUserService
{
    Task<Result<IEnumerable<UserVM>>> GetUsers();

    Task<Result<UserVM>> GetUserById(string Id);

    Task<Result> UpdateUser(UserRequest userRequest);

    Task<Result<UserVM>> CreateUser(UserRequest userRequest);

    Task<Result> DeleteUser(string Id);

    Task<Result> ResetPassword(string Id);

    Task<Result> ChangePassword(ChangePasswordRequest changePasswordRequest);

    Task<Result<AccessControlVm>> GetAccessControlConfiguration();

    Task<Result> UpdateAccessControlConfiguration(RoleRequest roleRequest);
}

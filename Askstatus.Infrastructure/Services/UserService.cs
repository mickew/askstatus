using System.Security.Claims;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Users;
using Askstatus.Common.Users;
using Askstatus.Domain;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Askstatus.Infrastructure.Services;
public partial class UserService : IUserService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<UserService> _logger;
    private readonly IOptions<AskstatusApiSettings> _apiOptons;

    public UserService(SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<UserService> logger, IOptions<AskstatusApiSettings> apiOptons)
    {
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
        _apiOptons = apiOptons;
    }

    public async Task<Result> ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        var userId = _signInManager.Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = await _signInManager.UserManager.FindByIdAsync(userId!);
        if (user is null)
        {
            _logger.LogWarning("User with Id {Id} not found", userId);
            return Result.Fail(new IdentityNotFoundError("User not found"));
        }
        if (user.UserName == DbInitializer.DefaultAdminUserName)
        {
            _logger.LogWarning("Cannot change password for administrator {UserName} user", user.UserName);
            return Result.Fail(new IdentityBadRequestError("Cannot change password for admin user"));
        }

        var result = await _signInManager.UserManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
        if (result.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning("Could not change password for user {User}", user.UserName);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail(new IdentityBadRequestError("Could not change password", result.Errors));
    }

    public async Task<Result> ConfirmEmail(string Id, string Token)
    {
        var user = await _signInManager.UserManager.FindByIdAsync(Id);
        if (user is null)
        {
            _logger.LogWarning("User with Id {Id} not found", Id);
            return Result.Fail(new IdentityNotFoundError("User not found"));
        }
        if (user.UserName == DbInitializer.DefaultAdminUserName)
        {
            _logger.LogWarning("Cannot confirm email for administrator {UserName} user", user.UserName);
            return Result.Fail(new IdentityBadRequestError("Cannot confirm email for admin user"));
        }
        var result = await _signInManager.UserManager.ConfirmEmailAsync(user, Token);
        if (result.Succeeded)
        {
            return Result.Ok();
        }
        else
        {
            _logger.LogWarning("Could not confirm email for user {User}", user.UserName);
            foreach (var error in result.Errors)
            {
                _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
            }
            return Result.Fail(new IdentityBadRequestError("Could not confirm email", result.Errors));
        }
    }

    public async Task<Result<RoleDto>> CreateRole(RoleRequest roleRequest)
    {
        var role = new ApplicationRole
        {
            Name = roleRequest.Name,
            Permissions = roleRequest.Permission
        };
        var result = await _roleManager.CreateAsync(role);
        if (result.Succeeded)
        {
            return Result.Ok(new RoleDto(role.Id, role.Name, role.Permissions));
        }
        _logger.LogWarning("Could not create role {Role}", roleRequest.Name);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail<RoleDto>(new IdentityBadRequestError("Could not create role", result.Errors));
    }

    public async Task<Result<UserVMWithLink>> CreateUser(UserRequest userRequest)
    {
        var user = new ApplicationUser
        {
            UserName = userRequest.UserName,
            Email = userRequest.Email,
            FirstName = userRequest.FirstName,
            LastName = userRequest.LastName
        };
        var result = await _signInManager.UserManager.CreateAsync(user, $"!1{char.ToUpper(userRequest.UserName![0])}{userRequest.UserName.Substring(1)}1!");
        if (result.Succeeded)
        {
            var token = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                { "userId", user.Id },
                { "token", token }
            };
            var callback = QueryHelpers.AddQueryString($"{_apiOptons.Value.FrontendUrl}/confirm-email", param);
            result = await _signInManager.UserManager.AddToRolesAsync(user, userRequest.Roles);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Could not add roles to user {User}", userRequest.UserName);
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
                }
                return Result.Fail<UserVMWithLink>(new IdentityBadRequestError("Could not add roles to user", result.Errors));
            }
            return Result.Ok(new UserVMWithLink(user.Id, user.UserName!, user.Email!, user.FirstName!, user.LastName!, callback));
        }
        _logger.LogWarning("Could not create user {User}", userRequest.UserName);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail<UserVMWithLink>(new IdentityBadRequestError("Culd not create user", result.Errors));
    }

    public async Task<Result> DeleteRole(string Id)
    {
        var role = await _roleManager.FindByIdAsync(Id);
        if (role is null)
        {
            _logger.LogWarning("Role with Id {Id} not found", Id);
            return Result.Fail(new IdentityNotFoundError("Role not found"));
        }
        if (role.Name == DbInitializer.AdministratorsRole)
        {
            _logger.LogWarning("Cannot delete default administrator {UserName} role", role.Name);
            return Result.Fail(new IdentityBadRequestError("Cannot delete default administrator role"));
        }

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning("Could not delete role {Role}", role.Name);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail(new IdentityBadRequestError("Could not delete role", result.Errors));
    }

    public async Task<Result> DeleteUser(string Id)
    {
        var result = await _signInManager.UserManager.FindByIdAsync(Id);
        if (result is null)
        {
            _logger.LogWarning("User with Id {Id} not found", Id);
            return Result.Fail(new IdentityNotFoundError("User not found"));
        }
        if (result.UserName == DbInitializer.DefaultAdminUserName)
        {
            _logger.LogWarning("Cannot delete default administrator {UserName} user", result.UserName);
            return Result.Fail(new IdentityBadRequestError("Cannot delete default admin user"));
        }
        var deleteResult = await _signInManager.UserManager.DeleteAsync(result);
        if (deleteResult.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning("Could not delete user {User}", result.UserName);
        foreach (var error in deleteResult.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail(new IdentityBadRequestError("Could not delete user", deleteResult.Errors));
    }

    public async Task<Result<UserVMWithLink>> ForgotPassword(string email)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(email).ConfigureAwait(false);
        if (user is not null)
        {
            var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var param = new Dictionary<string, string?>
            {
                { "userId", user.Id },
                { "token", token }
            };
            var callback = QueryHelpers.AddQueryString($"{_apiOptons.Value.FrontendUrl}/reset-password", param);
            _logger.LogInformation("Password reset token generated for user {User}", email);
            return Result.Ok(new UserVMWithLink(user.Id, user.UserName!, user.Email!, user.FirstName!, user.LastName!, callback));
        }
        _logger.LogWarning("User with email {Email} not found", email);
        return Result.Fail<UserVMWithLink>(new IdentityNotFoundError("User not found"));
    }

    public async Task<Result<AccessControlVm>> GetAccessControlConfiguration()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleDtos = roles
            .Select(r => new RoleDto(r.Id, r.Name ?? string.Empty, r.Permissions))
            .OrderBy(r => r.Name)
            .ToList();
        return Result.Ok(new AccessControlVm(roleDtos));
    }

    public async Task<Result<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .ToListAsync();
        return Result.Ok(roles.Select(r => new RoleDto(r.Id, r.Name ?? string.Empty, r.Permissions)).AsEnumerable());
    }

    public async Task<Result<UserVM>> GetUserById(string Id)
    {
        var result = await _signInManager.UserManager.Users.FirstOrDefaultAsync(u => u.Id == Id);
        if (result is null)
        {
            _logger.LogWarning("User with Id {Id} not found", Id);
            return Result.Fail<UserVM>(new IdentityNotFoundError("User not found"));
        }
        UserVM user = new(result.Id, result.UserName!, result.Email!, result.FirstName!, result.LastName!);
        var roles = await _signInManager.UserManager.GetRolesAsync(result);
        if (roles is null)
        {
            _logger.LogWarning("Could not get any roles");
            return Result.Fail<UserVM>(new IdentityBadRequestError("Could not get any roles"));
        }
        user.Roles.AddRange(roles);
        return Result.Ok(user);
    }

    public async Task<Result<IEnumerable<UserVM>>> GetUsers()
    {
        var result = await _signInManager.UserManager.Users.OrderBy(r => r.UserName)
                                       .Select(u => new UserVM(u.Id, u.UserName!, u.Email!, u.FirstName!, u.LastName!))
                                       .ToListAsync();
        return Result.Ok(result.AsEnumerable());
    }

    public async Task<Result> ResetPassword(string Id)
    {
        var result = await _signInManager.UserManager.FindByIdAsync(Id);
        if (result is null)
        {
            _logger.LogWarning("User with Id {Id} not found", Id);
            return Result.Fail(new IdentityNotFoundError("User not found"));
        }
        if (result.UserName == DbInitializer.DefaultAdminUserName)
        {
            _logger.LogWarning("Cannot reset password for administrator {UserName} user", result.UserName);
            return Result.Fail(new IdentityBadRequestError("Cannot reset password for admin user"));
        }

        var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(result);
        var resetResult = await _signInManager.UserManager.ResetPasswordAsync(result, token, $"!1{char.ToUpper(result.UserName![0])}{result.UserName.Substring(1)}1!");
        if (resetResult.Succeeded)
        {
            //result.AccessFailedCount = 0;
            //result.LockoutEnd = null;
            //await _signInManager.UserManager.UpdateAsync(result);
            return Result.Ok();
        }
        _logger.LogWarning("Could not reset password for user {User}", result.UserName);
        foreach (var error in resetResult.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail(new IdentityBadRequestError("Could not reset password", resetResult.Errors));
    }

    public async Task<Result> ResetUserPassword(string Id, string Token, string NewPassword)
    {
        var user = await _signInManager.UserManager.FindByIdAsync(Id);
        if (user is null)
        {
            _logger.LogWarning("User with email {Id} not found", Id);
            return Result.Fail(new IdentityNotFoundError("User not found"));
        }
        var result = await _signInManager.UserManager.ResetPasswordAsync(user, Token, NewPassword);
        if (result.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning("Could not reset password for user {User}", user.UserName);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail(new IdentityBadRequestError("Could not reset password", result.Errors));
    }

    public async Task<Result> UpdateAccessControlConfiguration(RoleRequest roleRequest)
    {
        var role = await _roleManager.FindByIdAsync(roleRequest.Id);
        if (role is null)
        {
            _logger.LogWarning("Role with Id {Id} not found", roleRequest.Id);
            return Result.Fail(new IdentityNotFoundError("Role not found"));
        }
        if (role.Name == DbInitializer.AdministratorsRole)
        {
            _logger.LogWarning("Cannot update default administrator {UserName} role", role.Name);
            return Result.Fail(new IdentityBadRequestError("Cannot update default administrator role"));
        }

        role.Permissions = roleRequest.Permission;
        var result = await _roleManager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning("Could not update role {Role}", role.Name);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail(new IdentityBadRequestError("Could not update role", result.Errors));
    }

    public async Task<Result> UpdateRole(RoleRequest roleRequest)
    {
        var role = await _roleManager.FindByIdAsync(roleRequest.Id);
        if (role is null)
        {
            _logger.LogWarning("Role with Id {Id} not found", roleRequest.Id);
            return Result.Fail(new IdentityNotFoundError("Role not found"));
        }
        if (role.Name == DbInitializer.AdministratorsRole)
        {
            _logger.LogWarning("Cannot update default administrator {UserName} role", role.Name);
            return Result.Fail(new IdentityBadRequestError("Cannot update default administrator role"));
        }

        role.Name = roleRequest.Name;

        var result = await _roleManager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning("Could not update role {Role}", role.Name);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail(new IdentityBadRequestError("Could not update role", result.Errors));
    }

    public async Task<Result> UpdateUser(UserRequest userRequest)
    {
        var user = await _signInManager.UserManager.FindByIdAsync(userRequest.Id);
        if (user is null)
        {
            _logger.LogWarning("User with Id {Id} not found", userRequest.Id);
            return Result.Fail(new IdentityNotFoundError("User not found"));
        }
        if (user.UserName == DbInitializer.DefaultAdminUserName)
        {
            _logger.LogWarning("Cannot update default administrator {UserName} user", user.UserName);
            return Result.Fail(new IdentityBadRequestError("Cannot update default admin user"));
        }

        user.UserName = userRequest.UserName;
        user.Email = userRequest.Email;
        user.FirstName = userRequest.FirstName;
        user.LastName = userRequest.LastName;
        var result = await _signInManager.UserManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            var curentRoles = await _signInManager.UserManager.GetRolesAsync(user);
            var addedRoles = userRequest.Roles.Except(curentRoles ?? new List<string>());
            var removedRoles = new List<string>();
            if (curentRoles is not null)
            {
                removedRoles = curentRoles.Except(userRequest.Roles).ToList();
            }

            if (addedRoles.Any())
            {
                result = await _signInManager.UserManager.AddToRolesAsync(user, addedRoles);
            }

            if (removedRoles.Any() && result.Succeeded)
            {
                result = await _signInManager.UserManager.RemoveFromRolesAsync(user, removedRoles);
            }

            if (result.Succeeded)
            {
                return Result.Ok();
            }
        }
        _logger.LogWarning("Could not update user {User}", user.UserName);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        //return Result.Fail("Could not update user");
        return Result.Fail(new IdentityBadRequestError("Could not update user", result.Errors));
    }
}

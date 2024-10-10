using System.Security.Claims;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Askstatus.Infrastructure.Services;
public class UserService : IUserService
{
    //private readonly UserManager<ApplicationUser> _signInManager.UserManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<UserService> _logger;

    public UserService(SignInManager<ApplicationUser> signInManager, ILogger<UserService> logger)
    {
        //_signInManager.UserManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<Result> ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        var userId = _signInManager.Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var user = await _signInManager.UserManager.FindByIdAsync(userId!);
        if (user is null)
        {
            _logger.LogWarning("User with Id {Id} not found", userId);
            return Result.Fail("User not found");
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
        return Result.Fail("Could not change password");
    }

    public async Task<Result<UserVM>> CreateUser(UserRequest userRequest)
    {
        var user = new ApplicationUser
        {
            UserName = userRequest.UserName,
            Email = userRequest.Email,
            FirstName = userRequest.FirstName,
            LastName = userRequest.LastName
        };
        var result = await _signInManager.UserManager.CreateAsync(user, userRequest.UserName);
        if (result.Succeeded)
        {
            return Result.Ok(new UserVM(user.Id, user.UserName!, user.Email!, user.FirstName!, user.LastName!));
        }
        _logger.LogWarning("Could not create user {User}", userRequest.UserName);
        foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail<UserVM>("Culd not create user");
    }

    public async Task<Result> DeleteUser(string Id)
    {
        var result = await _signInManager.UserManager.FindByIdAsync(Id);
        if (result is null)
        {
            _logger.LogWarning("User with Id {Id} not found", Id);
            return Result.Fail("User not found");
        }
        if (result.UserName == DbInitializer.DefaultAdminUserName)
        {
            _logger.LogWarning("Cannot delete default administrator {UserName} user", result.UserName);
            return Result.Fail("Cannot delete default admin user");
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
        return Result.Fail("Could not delete user");
    }

    public async Task<Result<UserVM>> GetUserById(string Id)
    {
        var result = await _signInManager.UserManager.Users.FirstOrDefaultAsync(u => u.Id == Id);
        if (result is null)
        {
            _logger.LogWarning("User with Id {Id} not found", Id);
            return Result.Fail<UserVM>("User not found");
        }
        return Result.Ok(new UserVM(result!.Id, result.UserName!, result.Email!, result.FirstName!, result.LastName!));
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
            return Result.Fail("User not found");
        }

        var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(result);
        var resetResult = await _signInManager.UserManager.ResetPasswordAsync(result, token, $"!1{char.ToUpper(result.UserName![0])}{result.UserName.Substring(1)}1!");
        if (resetResult.Succeeded)
        {
            return Result.Ok();
        }
        _logger.LogWarning("Could not reset password for user {User}", result.UserName);
        foreach (var error in resetResult.Errors)
        {
            _logger.LogWarning("Error: {Error} | Code: {Code}", error.Description, error.Code);
        }
        return Result.Fail("Could not reset password");
    }

    public async Task<Result> UpdateUser(UserRequest userRequest)
    {
        var user = await _signInManager.UserManager.FindByIdAsync(userRequest.Id);
        if (user is null)
        {
            _logger.LogWarning("User with Id {Id} not found", userRequest.Id);
            return Result.Fail("User not found");
        }
        return Result.Ok();
    }
}

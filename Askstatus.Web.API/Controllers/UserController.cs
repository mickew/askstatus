using System.Net;
using Askstatus.Application.Authorization;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Permissions.ViewUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var result = await _userService.GetUsers();
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Permissions.ViewUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _userService.GetUserById(id);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Authorize(Permissions.ManageUsers)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody] UserRequest request)
    {
        var result = await _userService.CreateUser(request);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Authorize(Permissions.ManageUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] UserRequest request)
    {
        var result = await _userService.UpdateUser(request);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Permissions.ManageUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteUser(id);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Route("{id}/reset-password")]
    [Authorize(Permissions.ManageUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var result = await _userService.ResetPassword(id);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Route("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePassword(request);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

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
public class RoleController : ControllerBase
{
    private readonly IUserService _userService;

    public RoleController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Permissions.ViewRoles)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var result = await _userService.GetRoles();
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Authorize(Permissions.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody] RoleRequest request)
    {
        var result = await _userService.CreateRole(request);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(Permissions.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Put([FromBody] RoleRequest request)
    {
        var result = await _userService.UpdateRole(request);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Permissions.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteRole(id);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Route("{id}/permissions")]
    [Authorize(Permissions.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PutPermissions([FromBody] RoleRequest request)
    {
        var result = await _userService.UpdateAccessControlConfiguration(request);
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet]
    [Route("{id}/permissions")]
    [Authorize(Permissions.ViewRoles)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _userService.GetAccessControlConfiguration();
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

using Askstatus.Application.Authorization;
using Askstatus.Application.Users;
using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly ISender _sender;

    public RoleController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Permissions.ViewRoles | Permissions.ManageUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var result = await _sender.Send(new GetRolesQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Authorize(Permissions.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody] RoleRequest request)
    {
        var result = await _sender.Send(new CreateRoleCommand { Name = request.Name, Permissions = request.Permission });
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Authorize(Permissions.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Put([FromBody] RoleRequest request)
    {
        var result = await _sender.Send(new UpdateRoleCommand { Id = request.Id, Name = request.Name, Permissions = request.Permission });
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
        var result = await _sender.Send(new DeleteRoleCommand(id));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Route("permission")]
    [Authorize(Permissions.ConfigureAccessControl)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PutPermissions([FromBody] RoleRequest request)
    {
        var result = await _sender.Send(new UpdateAccessControlConfigurationCommand { Id = request.Id, Permission = request.Permission });
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet]
    [Route("permissions")]
    [Authorize(Permissions.ViewAccessControl)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _sender.Send(new GetAccessControlConfigurationQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

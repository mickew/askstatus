using Askstatus.Application.Authorization;
using Askstatus.Application.DiscoverDevice;
using Askstatus.Common.Authorization;
using Askstatus.Common.Models;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeviceDiscoverController : ControllerBase
{
    private readonly ISender _sender;

    public DeviceDiscoverController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Permissions.DiscoverDevices)]
    [ProducesResponseType(typeof(DicoverInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(string host)
    {
        var result = await _sender.Send(new ShellyDiscoverDeviceQuery(host));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet("all")]
    [Authorize(Permissions.DiscoverDevices)]
    [ProducesResponseType(typeof(IEnumerable<DicoverInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new DiscoverAllDevicesQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet("notassigned")]
    [Authorize(Permissions.DiscoverDevices)]
    [ProducesResponseType(typeof(IEnumerable<DicoverInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotAssigned()
    {
        var result = await _sender.Send(new DiscoverDevicesQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

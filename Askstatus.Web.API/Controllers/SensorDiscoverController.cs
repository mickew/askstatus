using Askstatus.Application.Authorization;
using Askstatus.Application.Sensors;
using Askstatus.Common.Authorization;
using Askstatus.Common.Sensor;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SensorDiscoverController : ControllerBase
{
    private readonly ISender _sender;

    public SensorDiscoverController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("all")]
    [Authorize(Permissions.DiscoverSensors)]
    [ProducesResponseType(typeof(IEnumerable<SensorInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new DiscoverAllSensorsQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet("notassigned")]
    [Authorize(Permissions.DiscoverSensors)]
    [ProducesResponseType(typeof(IEnumerable<SensorInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotAssigned()
    {
        var result = await _sender.Send(new DiscoverSensorQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

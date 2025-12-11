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
public class SensorController : ControllerBase
{
    private readonly ISender _sender;

    public SensorController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var result = await _sender.Send(new GetSensorsQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Permissions.ViewSensors)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _sender.Send(new GetSensorByIdQuery(id));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Authorize(Permissions.ConfigureSensors)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] SensorRequest request)
    {
        var result = await _sender.Send(new CreateSensorCommand
        {
            Name = request.Name,
            SensorType = request.SensorType,
            FormatString = request.FormatString,
            SensorName = request.SensorName,
            SensorModel = request.SensorModel,
            ValueName = request.ValueName
        });
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Authorize(Permissions.ConfigureSensors)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] SensorRequest request)
    {
        var result = await _sender.Send(new UpdateSensorCommand
        {
            Id = request.Id,
            Name = request.Name,
            SensorType = request.SensorType,
            FormatString = request.FormatString,
            SensorName = request.SensorName,
            SensorModel = request.SensorModel,
            ValueName = request.ValueName
        });
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Permissions.ConfigureSensors)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sender.Send(new DeleteSensorCommand(id));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    //TODO: Implement tests for value
    [HttpGet]
    [Route("{id}/value")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSensorValue(int id)
    {
        var result = await _sender.Send(new GetSensorValueQuery(id));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

}


using Askstatus.Application.Authorization;
using Askstatus.Application.PowerDevice;
using Askstatus.Common.Authorization;
using Askstatus.Common.PowerDevice;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PowerDeviceController : ControllerBase
{
    private readonly ISender _sender;

    public PowerDeviceController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var result = await _sender.Send(new GetPowerDevicesQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Permissions.ViewPowerDevices)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _sender.Send(new GetPowerDeviceByIdQuery(id));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Authorize(Permissions.ConfigurePowerDevices)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] PowerDeviceRequest request)
    {
        var result = await _sender.Send(new CreatePowerDeviceCommand
        {
            Name = request.Name,
            DeviceType = request.DeviceType,
            HostName = request.HostName,
            DeviceMac = request.DeviceMac,
            DeviceName = request.DeviceName,
            DeviceModel = request.DeviceModel,
            DeviceId = request.DeviceId,
            Channel = request.Channel
        });
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPut]
    [Authorize(Permissions.ConfigurePowerDevices)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put([FromBody] PowerDeviceRequest request)
    {
        var result = await _sender.Send(new UpdatePowerDeviceCommand
        {
            Id = request.Id,
            Name = request.Name,
            DeviceType = request.DeviceType,
            HostName = request.HostName,
            DeviceMac = request.DeviceMac,
            DeviceName = request.DeviceName,
            DeviceModel = request.DeviceModel,
            DeviceId = request.DeviceId,
            Channel = request.Channel,
            ChanelType = request.ChanelType
        });
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Permissions.ConfigurePowerDevices)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sender.Send(new DeletePowerDeviceCommand(id));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    //TODO: Implement tests for state
    [HttpGet]
    [Route("{id}/state")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetState(int id)
    {
        var result = await _sender.Send(new GetPowerDeviceStateQuery(id));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet]
    [Route("{id}/toggle")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Toggle(int id)
    {
        var userName = HttpContext.User.Identity!.Name;
        var result = await _sender.Send(new TogglePowerDeviceCommand(id, userName!));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet]
    [Route("{id}/switch/{onoff}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Switch(int id, bool onoff)
    {
        var userName = HttpContext.User.Identity!.Name;
        var result = await _sender.Send(new SwitchPowerDeviceCommand(id, onoff, userName!));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    //TODO: Implement tests for webhook
    [HttpGet]
    [Route("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Webhook([FromQuery] string? mac, [FromQuery] string? state)
    {
        if (string.IsNullOrEmpty(mac) || string.IsNullOrEmpty(state))
        {
            return BadRequest();
        }
        var boolState = Convert.ToBoolean(state);
        var result = await _sender.Send(new PowerDeviceWebhookQuery(mac!, boolState));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Route("refresh-status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshStatus()
    {
        var result = await _sender.Send(new RefreshPowerDeviceStatusCommand());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Route("refresh")]
    [Authorize(Permissions.ConfigurePowerDevices)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh()
    {
        var result = await _sender.Send(new RefreshPowerDevicesCommand());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

}

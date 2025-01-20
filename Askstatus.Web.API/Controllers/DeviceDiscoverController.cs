using Askstatus.Application.Authorization;
using Askstatus.Application.DiscoverDevice;
using Askstatus.Common.Authorization;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(string host)
    {
        var result = await _sender.Send(new ShellyDiscoverDeviceQuery(host));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

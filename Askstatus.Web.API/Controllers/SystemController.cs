using Askstatus.Application.Authorization;
using Askstatus.Application.System;
using Askstatus.Common.Authorization;
using Askstatus.Common.System;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SystemController : ControllerBase
{
    private readonly ISender _sender;

    public SystemController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Route("systemlog")]
    [Authorize(Permissions.System)]
    [ProducesResponseType(typeof(SystemLogPagedList), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(string? searchTerm, string? sortColumn, int page = 1, int pageSize = 10, bool desc = false)
    {
        var result = await _sender.Send(new GetSystemLogsQuery(searchTerm, sortColumn, page, pageSize, desc));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost]
    [Route("uploadgoogletokenresponsefile")]
    [Authorize(Permissions.System)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadGoogleTokenResponseFile(IFormFile file)
    {
        using (var fileStream = file.OpenReadStream())
        {
            var command = new UploadGoogleTokenResponseFileCommand(file.FileName, fileStream);
            var result = await _sender.Send(command);
            return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
        }
    }

    [HttpPost]
    [Route("uploadproductionappsettingsfile")]
    [Authorize(Permissions.System)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadProductionAppSettingsFile(IFormFile file)
    {
        using (var fileStream = file.OpenReadStream())
        {
            var command = new UploadProductionAppSettingsFileCommand(file.FileName, fileStream);
            var result = await _sender.Send(command);
            return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
        }
    }
}

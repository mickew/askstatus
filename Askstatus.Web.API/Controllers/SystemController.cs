using Askstatus.Application.System;
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

    //TODO: Implement tests for Get
    [HttpGet]
    [Route("systemlog")]
    //[Authorize(Permissions.ViewSystemLogs)]
    [ProducesResponseType(typeof(SystemLogPagedList), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(string? searchTerm, string? sortColumn, int page = 1, int pageSize = 10, bool desc = false)
    {
        var result = await _sender.Send(new GetSystemLogsQuery(searchTerm, sortColumn, page, pageSize, desc));
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

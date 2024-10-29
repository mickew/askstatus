using System.Net;
using Askstatus.Application.Authorization;
using Askstatus.Application.Identity;
using Askstatus.Common.Identity;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Askstatus.Web.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<IdentityController> _logger;

    public IdentityController(ISender sender, ILogger<IdentityController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest login)
    {
        Result result = await _sender.Send(new LoginUserCommand { UserName = login.UserName, Password = login.Password });
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var result = await _sender.Send(new LogOutUserCommand());
        return result.ToActionResult();
    }

    [HttpGet("userinfo")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserInfo()
    {
        var result = await _sender.Send(new GetUserInfoQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }

    [HttpGet("claims")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetApplicationClaims()
    {
        var result = await _sender.Send(new GetApplicationClaimsQuery());
        return result.ToActionResult(new AskstatusAspNetCoreResultEndpointProfile());
    }
}

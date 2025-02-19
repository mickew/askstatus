using Askstatus.Common.System;
using Askstatus.Domain;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace Askstatus.Application.System;

public sealed record GetSystemInfoQuery() : IRequest<Result<SystemInfoDto>>;

public sealed class GetSystemInfoQueryHandler : IRequestHandler<GetSystemInfoQuery, Result<SystemInfoDto>>
{
    private readonly IOptions<MailSettings> _mailOptions;
    private readonly IOptions<AskstatusApiSettings> _apiOptions;

    public GetSystemInfoQueryHandler(IOptions<MailSettings> mailOptions, IOptions<AskstatusApiSettings> apiOptions)
    {
        _mailOptions = mailOptions;
        _apiOptions = apiOptions;
    }

    public Task<Result<SystemInfoDto>> Handle(GetSystemInfoQuery request, CancellationToken cancellationToken)
    {
        var mailSettings = _mailOptions.Value;
        var apiSettings = _apiOptions.Value;
        var mailSettingsDto = new SystemMailSettingsDto(
            mailSettings.Enabled,
            mailSettings.Host!,
            mailSettings.Port,
            mailSettings.Account!,
            mailSettings.Password!,
            mailSettings.ClientId!,
            mailSettings.ClientSecret!,
            mailSettings.EnableSsl,
            mailSettings.CredentialCacheFolder!);
        var apiSettingsDto = new SystemApiSettingsDto(
            apiSettings.BackendUrl!,
            apiSettings.FrontendUrl!);
        return Task.FromResult(Result.Ok(new SystemInfoDto(mailSettingsDto, apiSettingsDto)));
    }
}

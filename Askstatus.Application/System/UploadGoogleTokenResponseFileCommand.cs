using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Domain;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Askstatus.Application.System;
public sealed record UploadGoogleTokenResponseFileCommand(string FileName, Stream Data) : IRequest<Result>;

public sealed class UploadGoogleTokenResponseFileCommandHandler : IRequestHandler<UploadGoogleTokenResponseFileCommand, Result>
{
    private readonly IFileService _fileService;
    private readonly ILogger<UploadGoogleTokenResponseFileCommandHandler> _logger;
    private readonly IOptionsSnapshot<MailSettings> _options;

    public UploadGoogleTokenResponseFileCommandHandler(IFileService fileService, ILogger<UploadGoogleTokenResponseFileCommandHandler> logger, IOptionsSnapshot<MailSettings> options)
    {
        _fileService = fileService;
        _logger = logger;
        _options = options;
    }
    public async Task<Result> Handle(UploadGoogleTokenResponseFileCommand request, CancellationToken cancellationToken)
    {
        var fileName = Path.Combine(_options.Value.CredentialCacheFolder!, request.FileName);
        var result = await _fileService.SaveFileAsync(fileName, request.Data, cancellationToken);
        if (!result)
        {
            _logger.LogError("Failed to save file with file name {filename}", request.FileName);
            return Result.Fail(new BadRequestError("Failed to save file"));
        }
        return Result.Ok();
    }
}

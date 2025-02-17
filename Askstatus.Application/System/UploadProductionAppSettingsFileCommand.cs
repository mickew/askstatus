using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.System;
public sealed record UploadProductionAppSettingsFileCommand(string FileName, Stream Data) : IRequest<Result>;

public sealed class UploadProductionAppSettingsFileCommandHandler : IRequestHandler<UploadProductionAppSettingsFileCommand, Result>
{
    private readonly IFileService _fileService;
    private readonly ILogger<UploadProductionAppSettingsFileCommandHandler> _logger;

    public UploadProductionAppSettingsFileCommandHandler(IFileService fileService, ILogger<UploadProductionAppSettingsFileCommandHandler> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }
    public async Task<Result> Handle(UploadProductionAppSettingsFileCommand request, CancellationToken cancellationToken)
    {
        var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, request.FileName);
        var result = await _fileService.SaveFileAsync(fileName, request.Data, cancellationToken);
        if (!result)
        {
            _logger.LogError("Failed to save file with file name {filename}", request.FileName);
            return Result.Fail(new BadRequestError("Failed to save file"));
        }
        return Result.Ok();
    }
}

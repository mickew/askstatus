using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Users;
public sealed record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result>;

public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;

    public ConfirmEmailCommandHandler(IUserService userService, IUnitOfWork unitOfWork, ILogger<ConfirmEmailCommandHandler> logger)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.ConfirmEmail(request.UserId, request.Token);
        if (result.IsSuccess)
        {
            try
            {
                var user = await _userService.GetUserById(request.UserId);
                await _unitOfWork.SystemLogRepository.AddAsync(new Askstatus.Domain.Entities.SystemLog
                {
                    EventTime = DateTime.UtcNow,
                    EventType = SystemLogEventType.ConfirmEmail,
                    User = user.Value.UserName,
                    Message = $"user with email {user.Value.Email} confirmed"
                });
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving SystemLog");
            }
        }
        return result;
    }
}

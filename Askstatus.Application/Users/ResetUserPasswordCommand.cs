using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Users;
public sealed record ResetUserPasswordCommand(string UserId, string Token, string NewPassword) : IRequest<Result>;

public sealed class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, Result>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResetUserPasswordCommandHandler> _logger;

    public ResetUserPasswordCommandHandler(IUserService userService, IUnitOfWork unitOfWork, ILogger<ResetUserPasswordCommandHandler> logger)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.ResetUserPassword(request.UserId, request.Token, request.NewPassword);
        if (result.IsSuccess)
        {
            try
            {
                var user = await _userService.GetUserById(request.UserId);
                await _unitOfWork.SystemLogRepository.AddAsync(new Askstatus.Domain.Entities.SystemLog
                {
                    EventTime = DateTime.UtcNow,
                    EventType = SystemLogEventType.ResetPassword,
                    User = user.Value.UserName,
                    Message = $"user reset password"
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

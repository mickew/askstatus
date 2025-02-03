using Askstatus.Application.Events;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserService _userService;
    private readonly IEventBus _eventBus;

    public ForgotPasswordCommandHandler(IUserService userService, IEventBus eventBus)
    {
        _userService = userService;
        _eventBus = eventBus;
    }
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.ForgotPassword(request.Email);
        if (result.IsSuccess)
        {
            await _eventBus.PublishAsync(new UserChangedIntegrationEvent(Guid.NewGuid(), result.Value, UserEventType.UserForgotPassword));
            return Result.Ok();
        }
        return Result.Fail(result.Errors.FirstOrDefault());
    }
}

using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserService _userService;
    private readonly IPublisher _publisher;

    public ForgotPasswordCommandHandler(IUserService userService, IPublisher publisher)
    {
        _userService = userService;
        _publisher = publisher;
    }
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.ForgotPassword(request.Email);
        if (result.IsSuccess)
        {
            await _publisher.Publish(new UserChangedEvent(result.Value, UserEventType.UserForgotPassword));
            return Result.Ok();
        }
        return Result.Fail(result.Errors.FirstOrDefault());
    }
}

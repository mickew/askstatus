using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record ResetUserPasswordCommand(string UserId, string Token, string NewPassword) : IRequest<Result>;

public sealed class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, Result>
{
    private readonly IUserService _userService;
    public ResetUserPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _userService.ResetUserPassword(request.UserId, request.Token, request.NewPassword);
    }
}

using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record ResetPasswordCommand(string Id) : IRequest<Result>;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserService _userService;

    public ResetPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.ResetPassword(request.Id);
        return result;
    }
}

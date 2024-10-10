using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record ChangePasswordCommand : IRequest<Result>
{
    public string? OldPassword { get; init; }
    public string? NewPassword { get; init; }
}

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserService _userService;

    public ChangePasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var changePasswordRequest = new ChangePasswordRequest(request.OldPassword!, request.NewPassword!);
        var result = await _userService.ChangePassword(changePasswordRequest);
        return result;
    }
}

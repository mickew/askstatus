using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record ConfirmEmailCommand(string UserId, string Code) : IRequest<Result>;

public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IUserService _userService;
    public ConfirmEmailCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await _userService.ConfirmEmail(request.UserId, request.Code);
    }
}

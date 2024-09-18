using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Identity;
public sealed record LogOutUserCommand : IRequest<Result>;

public sealed class LogOutUserCommandHandler : IRequestHandler<LogOutUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public LogOutUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(LogOutUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.Logout();
        return result;
    }
}

using Askstatus.Application.Interfaces;
using Askstatus.Application.Models.Identity;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Identity;
public sealed record LoginUserCommand : IRequest<Result>
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
}

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var loginDto = new LoginDto(request.UserName!, request.Password!);
        var result = await _identityService.Login(loginDto);
        return result;
    }
}

using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record UpdateUserCommand : IRequest<Result>
{
    public string? Id { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public List<string>? Roles { get; init; }
}

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userRequest = new UserRequest(request.Id!, request.UserName!, request.Email!, request.FirstName!, request.LastName!, request.Roles ?? new List<string>());
        var result = await _userService.UpdateUser(userRequest);
        return result;
    }
}

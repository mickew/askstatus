using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record DeleteRoleCommand(string Id) : IRequest<Result>;

public sealed class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IUserService _userService;

    public DeleteRoleCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteRole(request.Id);
        return result;
    }
}

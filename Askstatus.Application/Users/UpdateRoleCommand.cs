using Askstatus.Application.Interfaces;
using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record UpdateRoleCommand : IRequest<Result>
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public Permissions Permissions { get; init; }
}

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IUserService _userService;

    public UpdateRoleCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleRequest = new RoleRequest(request.Id!, request.Name!, request.Permissions);
        var result = await _userService.UpdateRole(roleRequest);
        return result;
    }
}

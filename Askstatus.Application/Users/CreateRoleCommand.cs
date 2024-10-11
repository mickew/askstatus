using Askstatus.Application.Interfaces;
using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record CreateRoleCommand : IRequest<Result<RoleDto>>
{
    public string? Name { get; init; }
    public Permissions Permissions { get; init; }
}

public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    private readonly IUserService _userService;

    public CreateRoleCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleRequest = new RoleRequest(string.Empty, request.Name!, request.Permissions);
        var result = await _userService.CreateRole(roleRequest);
        return result;
    }
}

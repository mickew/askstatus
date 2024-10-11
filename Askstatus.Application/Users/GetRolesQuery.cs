using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;

public sealed record GetRolesQuery : IRequest<Result<IEnumerable<RoleDto>>>;

public sealed class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<IEnumerable<RoleDto>>>
{
    private readonly IUserService _userService;

    public GetRolesQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<IEnumerable<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetRoles();
        return result;
    }
}

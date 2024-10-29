using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record GetUsersQuery : IRequest<Result<IEnumerable<UserVM>>>;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserVM>>>
{
    private readonly IUserService _userService;

    public GetUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<IEnumerable<UserVM>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUsers();
        return result;
    }
}

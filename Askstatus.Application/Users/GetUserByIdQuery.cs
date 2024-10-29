using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record GetUserByIdQuery(string Id) : IRequest<Result<UserVM>>;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserVM>>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<UserVM>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserById(request.Id);
        return result;
    }
}

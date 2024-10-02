using Askstatus.Application.Interfaces;
using Askstatus.Common.Identity;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Identity;
public sealed record GetUserInfoQuery : IRequest<Result<UserInfoVM>>;

public sealed class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Result<UserInfoVM>>
{
    private readonly IIdentityService _identityService;

    public GetUserInfoQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserInfoVM>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUserInfo();
        return result;
    }
}

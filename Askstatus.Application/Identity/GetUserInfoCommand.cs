using Askstatus.Application.Interfaces;
using Askstatus.Application.Models.Identity;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Identity;
public sealed record GetUserInfoCommand : IRequest<Result<UserInfoDto>>;

public sealed class GetUserInfoCommandHandler : IRequestHandler<GetUserInfoCommand, Result<UserInfoDto>>
{
    private readonly IIdentityService _identityService;

    public GetUserInfoCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserInfoDto>> Handle(GetUserInfoCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUserInfo();
        return result;
    }
}

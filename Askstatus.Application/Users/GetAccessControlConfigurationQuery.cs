using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;

public sealed record GetAccessControlConfigurationQuery : IRequest<Result<AccessControlVm>>;

public sealed class GetAccessControlConfigurationQueryHandler : IRequestHandler<GetAccessControlConfigurationQuery, Result<AccessControlVm>>
{
    private readonly IUserService _userService;

    public GetAccessControlConfigurationQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<AccessControlVm>> Handle(GetAccessControlConfigurationQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetAccessControlConfiguration();
        return result;
    }
}

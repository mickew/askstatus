using Askstatus.Application.Interfaces;
using Askstatus.Common.Identity;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Identity;
public sealed record GetApplicationClaimsQuery : IRequest<Result<IEnumerable<ApplicationClaimVM>>>;

public sealed class GetApplicationClaimsQueryHandler : IRequestHandler<GetApplicationClaimsQuery, Result<IEnumerable<ApplicationClaimVM>>>
{
    private readonly IIdentityService _identityService;

    public GetApplicationClaimsQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<IEnumerable<ApplicationClaimVM>>> Handle(GetApplicationClaimsQuery request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetApplicationClaims();
        return result;
    }
}

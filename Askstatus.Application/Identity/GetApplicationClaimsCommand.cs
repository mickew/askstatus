using Askstatus.Application.Interfaces;
using Askstatus.Application.Models.Identity;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Identity;
public sealed record GetApplicationClaimsCommand : IRequest<Result<IEnumerable<ApplicationClaimDto>>>;

public sealed class GetApplicationClaimsCommandHandler : IRequestHandler<GetApplicationClaimsCommand, Result<IEnumerable<ApplicationClaimDto>>>
{
    private readonly IIdentityService _identityService;

    public GetApplicationClaimsCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<IEnumerable<ApplicationClaimDto>>> Handle(GetApplicationClaimsCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetApplicationClaims();
        return result;
    }
}

using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using Askstatus.Common.Authorization;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record UpdateAccessControlConfigurationCommand : IRequest<Result>
{
    public string? Id { get; init; }
    public Permissions Permission { get; init; }
}

public sealed class UpdateAccessControlConfigurationCommandHandler : IRequestHandler<UpdateAccessControlConfigurationCommand, Result>
{
    private readonly IUserService _userService;

    public UpdateAccessControlConfigurationCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(UpdateAccessControlConfigurationCommand request, CancellationToken cancellationToken)
    {
        var roleRequest = new RoleRequest(request.Id!, string.Empty, request.Permission);
        var result = await _userService.UpdateAccessControlConfiguration(roleRequest);
        return result;
    }
}

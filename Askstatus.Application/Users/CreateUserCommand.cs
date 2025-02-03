using Askstatus.Application.Events;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Users;
public sealed record CreateUserCommand : IRequest<Result<UserVM>>
{
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public List<string>? Roles { get; init; }

}

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserVM>>
{
    private readonly IUserService _userService;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly IEventBus _eventBus;

    public CreateUserCommandHandler(IUserService userService, ILogger<CreateUserCommandHandler> logger, IEventBus eventBus)
    {
        _userService = userService;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<Result<UserVM>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userRequest = new UserRequest(string.Empty, request.UserName!, request.Email!, request.FirstName!, request.LastName!, request.Roles ?? new List<string>());
        var result = await _userService.CreateUser(userRequest);
        if (result.IsSuccess)
        {
            await _eventBus.PublishAsync(new UserChangedIntegrationEvent(Guid.NewGuid(), result.Value, UserEventType.UserCreated));
            return Result.Ok(result.Value as UserVM);
        }
        return Result.Fail<UserVM>(result.Errors.FirstOrDefault());
    }
}

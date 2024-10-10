﻿using Askstatus.Application.Interfaces;
using Askstatus.Common.Users;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record CreateUserCommand : IRequest<Result<UserVM>>
{
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserVM>>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<UserVM>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userRequest = new UserRequest(string.Empty, request.UserName!, request.Email!, request.FirstName!, request.LastName!);
        var result = await _userService.CreateUser(userRequest);
        return result;
    }
}
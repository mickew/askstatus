using Askstatus.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Askstatus.Application.Authorization;
public class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    public PermissionAuthorizationRequirement(Permissions permission)
    {
        Permissions = permission;
    }

    public Permissions Permissions { get; }
}

using Askstatus.Domain.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Askstatus.Infrastructure.Identity;
public class ApplicationRole : IdentityRole
{
    public Permissions Permissions { get; set; }
}
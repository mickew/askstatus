﻿using Microsoft.AspNetCore.Identity;

namespace Askstatus.Infrastructure.Identity;
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

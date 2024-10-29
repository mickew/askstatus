using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using FluentAssertions;

namespace Askstatus.Application.Tests;
public class RoleDtoTests
{
    [Fact]
    public void HasPermission()
    {
        // Arrange
        var role = new RoleDto("1", "Admin", Permissions.All);

        //Assert
        foreach (var permission in Enum.GetValues<Permissions>())
        {
            if (permission == Permissions.None) continue;
            if (permission == Permissions.All) continue;
            role.Has(permission).Should().BeTrue();
        }
    }

    [Fact]
    public void GrantPermission()
    {
        // Arrange
        var role = new RoleDto("1", "Admin", Permissions.None);

        // Act
        role.Set(Permissions.ViewUsers, true);

        // Assert
        role.Has(Permissions.ViewUsers).Should().BeTrue();
        foreach (var permission in Enum.GetValues<Permissions>())
        {
            if (permission == Permissions.None) continue;
            if (permission == Permissions.All) continue;
            if (permission == Permissions.ViewUsers) continue;
            role.Has(permission).Should().BeFalse();
        }
    }

    [Fact]
    public void RevokePermission()
    {
        var role = new RoleDto("1", "Admin", Permissions.All);
        role.Set(Permissions.ViewUsers, false);
        role.Has(Permissions.ViewUsers).Should().BeFalse();
        foreach (var permission in Enum.GetValues<Permissions>())
        {
            if (permission == Permissions.None) continue;
            if (permission == Permissions.All) continue;
            if (permission == Permissions.ViewUsers) continue;
            role.Has(permission).Should().BeTrue();
        }
    }
}

using System.Security.Claims;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Models.Identity;
using Askstatus.Domain.Constants;
using Askstatus.Infrastructure.Identity;
using Askstatus.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Askstatus.Infrastructure.Tests;
public class IdentityTests
{
    [Fact]
    public async Task Login_Should_Return_Success()
    {
        //Aranage
        IIdentityService identityService = new IdentityService(new FakeSignInManager(), new Mock<ILogger<IdentityService>>().Object);

        //Act
        var result = await identityService.Login(new LoginDto("admin", "admin"));

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Login_Should_Return_Failiure()
    {
        //Aranage
        IIdentityService identityService = new IdentityService(new FakeSignInManager(false), new Mock<ILogger<IdentityService>>().Object);

        //Act
        var result = await identityService.Login(new LoginDto("admin", "admin"));

        //Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Login failed");
        result.IsFailed.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Reasons.Should().NotBeEmpty();
        result.Reasons.Should().HaveCount(1);
        result.Reasons.First().Message.Should().Be("Login failed");
    }

    [Fact]
    public async Task GetUserInfo_Should_Return_Success()
    {
        //Aranage
        IIdentityService identityService = new IdentityService(new FakeSignInManager(), new Mock<ILogger<IdentityService>>().Object);

        //Act
        var result = await identityService.GetUserInfo();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserName.Should().Be("admin");
        result.Value.Email.Should().Be("admin@localhost");
        result.Value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUserInfo_Should_Return_Failiure()
    {
        //Aranage
        IIdentityService identityService = new IdentityService(new FakeSignInManager(false), new Mock<ILogger<IdentityService>>().Object);

        //Act
        var result = await identityService.GetUserInfo();

        //Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("User not found");
        result.IsFailed.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Reasons.Should().NotBeEmpty();
        result.Reasons.Should().HaveCount(1);
        result.Reasons.First().Message.Should().Be("User not found");
    }

    [Fact]
    public async Task LogOut_Should_Return_Success()
    {
        // Arrange
        IIdentityService identityService = new IdentityService(new FakeSignInManager(), new Mock<ILogger<IdentityService>>().Object);

        // Act
        var result = await identityService.Logout();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetApplicationClaims_Should_Return_Success()
    {
        // Arrange
        var expected = new List<ApplicationClaimDto>
            {
                new ApplicationClaimDto("LOCAL AUTHORITY", "LOCAL AUTHORITY", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "http://www.w3.org/2001/XMLSchema#string"),
                new ApplicationClaimDto("LOCAL AUTHORITY", "LOCAL AUTHORITY", "permissions", "-1", "http://www.w3.org/2001/XMLSchema#string"),
            };
        IIdentityService identityService = new IdentityService(new FakeSignInManager(), new Mock<ILogger<IdentityService>>().Object);

        // Act
        var result = await identityService.GetApplicationClaims();

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(expected.AsEnumerable());
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetApplicationClaims_Should_Return_Failiure()
    {
        // Arrange
        IIdentityService identityService = new IdentityService(new FakeSignInManager(false), new Mock<ILogger<IdentityService>>().Object);

        // Act
        var result = await identityService.GetApplicationClaims();

        // Assert
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Message.Should().Be("Not authorized");
        result.IsFailed.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Reasons.Should().NotBeEmpty();
        result.Reasons.Should().HaveCount(1);
        result.Reasons.First().Message.Should().Be("Not authorized");
    }
}

public class FakeUserManager : UserManager<ApplicationUser>
{
    public FakeUserManager()
        : base(new Mock<IUserStore<ApplicationUser>>().Object,
          new Mock<IOptions<IdentityOptions>>().Object,
          new Mock<IPasswordHasher<ApplicationUser>>().Object,
          new IUserValidator<ApplicationUser>[0],
          new IPasswordValidator<ApplicationUser>[0],
          new Mock<ILookupNormalizer>().Object,
          new Mock<IdentityErrorDescriber>().Object,
          new Mock<IServiceProvider>().Object,
          new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
    { }

    public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }

    public override Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return base.FindByEmailAsync(email);
    }

    public override Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity!.Name != "admin")
            return Task.FromResult<ApplicationUser?>(null);
        ApplicationUser user = new ApplicationUser
        {
            UserName = principal.Identity!.Name,
            Email = principal.FindFirstValue(ClaimTypes.Email)
        };
        return Task.FromResult<ApplicationUser?>(user);
    }
}

public class FakeHttpContextAccessor : IHttpContextAccessor
{
    public FakeHttpContextAccessor(bool simulateSuccess = true)
    {
        if (simulateSuccess)
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Email, "admin@localhost"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(CustomClaimTypes.Permissions, "-1")
                }, "someAuthTypeName"))
            };
        }
        else
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                }))
            };
        }
    }

    public HttpContext? HttpContext { get; set; }
}

public class FakeSignInManager : SignInManager<ApplicationUser>
{
    private readonly bool _simulateSuccess = false;

    public FakeSignInManager(bool simulateSuccess = true)
            : base(new FakeUserManager(),
                 new FakeHttpContextAccessor(simulateSuccess),
                 new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                 new Mock<IOptions<IdentityOptions>>().Object,
                 new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                 new Mock<IAuthenticationSchemeProvider>().Object,
                 new Mock<IUserConfirmation<ApplicationUser>>().Object)
    {
        this._simulateSuccess = simulateSuccess;
    }

    public override Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        return this.ReturnResult(this._simulateSuccess);
    }

    public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        return this.ReturnResult(this._simulateSuccess);
    }

    public override Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
    {
        return this.ReturnResult(this._simulateSuccess);
    }

    public override Task SignOutAsync()
    {
        return Task.CompletedTask;
    }

    private Task<SignInResult> ReturnResult(bool isSuccess = true)
    {
        SignInResult result = SignInResult.Success;

        if (!isSuccess)
            result = SignInResult.Failed;

        return Task.FromResult(result);
    }
}


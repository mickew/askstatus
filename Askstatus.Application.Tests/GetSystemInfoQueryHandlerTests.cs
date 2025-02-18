using Askstatus.Application.System;
using Askstatus.Common.System;
using Askstatus.Domain;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace Askstatus.Application.Tests;

public class GetSystemInfoQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnSystemInfoDto_WhenCalled()
    {
        // Arrange
        var mailSettings = new MailSettings
        {
            Enabled = true,
            Host = "smtp.example.com",
            Port = 587,
            Account = "user@example.com",
            Password = "password",
            ClientId = "clientId",
            ClientSecret = "clientSecret",
            EnableSsl = true,
            CredentialCacheFolder = "/path/to/cache"
        };

        var apiSettings = new AskstatusApiSettings
        {
            BackendUrl = "https://backend.example.com",
            FrontendUrl = "https://frontend.example.com"
        };

        var mailOptionsMock = new Mock<IOptions<MailSettings>>();
        mailOptionsMock.Setup(m => m.Value).Returns(mailSettings);

        var apiOptionsMock = new Mock<IOptions<AskstatusApiSettings>>();
        apiOptionsMock.Setup(m => m.Value).Returns(apiSettings);

        var handler = new GetSystemInfoQueryHandler(mailOptionsMock.Object, apiOptionsMock.Object);

        // Act
        var result = await handler.Handle(new GetSystemInfoQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.MailSettings.Should().BeEquivalentTo(new SystemMailSettingsDto(
            mailSettings.Enabled,
            mailSettings.Host,
            mailSettings.Port,
            mailSettings.Account,
            mailSettings.Password,
            mailSettings.ClientId,
            mailSettings.ClientSecret,
            mailSettings.EnableSsl,
            mailSettings.CredentialCacheFolder));
        result.Value.ApiSettings.Should().BeEquivalentTo(new SystemApiSettingsDto(
            apiSettings.BackendUrl,
            apiSettings.FrontendUrl));
    }
}

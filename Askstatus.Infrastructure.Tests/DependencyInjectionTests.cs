using Askstatus.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Quartz;

namespace Askstatus.Infrastructure.Tests;

public class DependencyInjectionTests
{
    private static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddLogging();
        return services;
    }

    private static IWebHostEnvironment CreateEnvironment()
    {
        var mock = new Mock<IWebHostEnvironment>();
        mock.SetupGet(e => e.ContentRootPath).Returns(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar));
        mock.SetupGet(e => e.EnvironmentName).Returns("Development");
        mock.SetupGet(e => e.ApplicationName).Returns("Askstatus.Tests");
        return mock.Object;
    }

    [Fact]
    public void AddInfrastructureServices_NullServices_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;
        var environment = CreateEnvironment();

        // Act
        var act = () => services.AddInfrastructureServices(environment);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddInfrastructureServices_ValidArguments_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = CreateServices();
        var environment = CreateEnvironment();

        // Act
        var result = services.AddInfrastructureServices(environment);

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersQuartzSchedulerFactory()
    {
        // Arrange
        var services = CreateServices();
        var environment = CreateEnvironment();

        // Act
        services.AddInfrastructureServices(environment);

        // Assert
        services.Should().Contain(d => d.ServiceType == typeof(ISchedulerFactory));
    }

    [Fact]
    public void AddInfrastructureServices_RegistersQuartzHostedService()
    {
        // Arrange
        var services = CreateServices();
        var environment = CreateEnvironment();

        // Act
        services.AddInfrastructureServices(environment);

        // Assert
        services.Should().Contain(d =>
            d.ServiceType == typeof(IHostedService) &&
            d.ImplementationType == typeof(QuartzHostedService));
    }

    [Fact]
    public void AddInfrastructureServices_ConfiguresQuartzHostedServiceOptions_WaitForJobsToCompleteIsTrue()
    {
        // Arrange
        var services = CreateServices();
        var environment = CreateEnvironment();

        // Act
        services.AddInfrastructureServices(environment);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<QuartzHostedServiceOptions>>().Value;

        // Assert
        options.WaitForJobsToComplete.Should().BeTrue();
    }

    [Fact]
    public void AddInfrastructureServices_WithoutRegistration_QuartzHostedServiceOptionsDefaultsToFalse()
    {
        // Arrange
        var services = CreateServices();

        // Act
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<QuartzHostedServiceOptions>>().Value;

        // Assert
        options.WaitForJobsToComplete.Should().BeFalse();
    }
}

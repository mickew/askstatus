using Askstatus.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Quartz;

namespace Askstatus.Infrastructure.Tests;

public class MapQuartsJobsTests
{
    private static IServiceProvider BuildProvider(IConfiguration configuration, ISchedulerFactory schedulerFactory)
    {
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddSingleton(schedulerFactory);
        return services.BuildServiceProvider();
    }

    private static IConfiguration BuildConfiguration(string? seconds)
    {
        var data = new Dictionary<string, string?>();
        if (seconds is not null)
        {
            data["AskstatusSettings:RunPublishToMqttJobEverySeconds"] = seconds;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
    }

    [Fact]
    public async Task MapJobs_WithConfiguredInterval_SchedulesJobWithThatInterval()
    {
        // Arrange
        var scheduler = new Mock<IScheduler>();
        var schedulerFactory = new Mock<ISchedulerFactory>();
        schedulerFactory.Setup(f => f.GetScheduler(It.IsAny<CancellationToken>())).ReturnsAsync(scheduler.Object);
        IJobDetail? capturedJob = null;
        ITrigger? capturedTrigger = null;
        scheduler.Setup(s => s.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<ScheduleJobOptions>(), It.IsAny<CancellationToken>()))
            .Callback<IJobDetail, ITrigger, ScheduleJobOptions, CancellationToken>((j, t, _, _) =>
            {
                capturedJob = j;
                capturedTrigger = t;
            })
            .ReturnsAsync(DateTimeOffset.UtcNow);
        var provider = BuildProvider(BuildConfiguration("15"), schedulerFactory.Object);

        // Act
        await MapQuartsJobs.MapJobs(provider);

        // Assert
        capturedJob.Should().NotBeNull();
        capturedJob!.Key.Name.Should().Be("PublishToMqttJob");
        capturedTrigger.Should().BeAssignableTo<ISimpleTrigger>();
        var simple = (ISimpleTrigger)capturedTrigger!;
        simple.RepeatInterval.Should().Be(TimeSpan.FromSeconds(15));
        simple.RepeatCount.Should().Be(SimpleTriggerImplConstants.RepeatIndefinitely);
        simple.JobKey.Should().Be(capturedJob.Key);
        scheduler.Verify(s => s.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<ScheduleJobOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        schedulerFactory.Verify(f => f.GetScheduler(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MapJobs_WithoutConfiguredInterval_UsesDefaultSixtySeconds()
    {
        // Arrange
        var scheduler = new Mock<IScheduler>();
        var schedulerFactory = new Mock<ISchedulerFactory>();
        schedulerFactory.Setup(f => f.GetScheduler(It.IsAny<CancellationToken>())).ReturnsAsync(scheduler.Object);
        ITrigger? capturedTrigger = null;
        scheduler.Setup(s => s.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<ScheduleJobOptions>(), It.IsAny<CancellationToken>()))
            .Callback<IJobDetail, ITrigger, ScheduleJobOptions, CancellationToken>((_, t, _, _) => capturedTrigger = t)
            .ReturnsAsync(DateTimeOffset.UtcNow);
        var provider = BuildProvider(BuildConfiguration(null), schedulerFactory.Object);

        // Act
        await MapQuartsJobs.MapJobs(provider);

        // Assert
        capturedTrigger.Should().BeAssignableTo<ISimpleTrigger>();
        ((ISimpleTrigger)capturedTrigger!).RepeatInterval.Should().Be(TimeSpan.FromSeconds(60));
        scheduler.Verify(s => s.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<ScheduleJobOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MapJobs_WhenSchedulerFactoryMissing_ThrowsAndDoesNotSchedule()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(BuildConfiguration("30"));
        var provider = services.BuildServiceProvider();

        // Act
        var act = async () => await MapQuartsJobs.MapJobs(provider);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task MapJobs_WhenConfigurationMissing_ThrowsAndDoesNotCallSchedulerFactory()
    {
        // Arrange
        var schedulerFactory = new Mock<ISchedulerFactory>();
        var services = new ServiceCollection();
        services.AddSingleton(schedulerFactory.Object);
        var provider = services.BuildServiceProvider();

        // Act
        var act = async () => await MapQuartsJobs.MapJobs(provider);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        schedulerFactory.Verify(f => f.GetScheduler(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static class SimpleTriggerImplConstants
    {
        public const int RepeatIndefinitely = -1;
    }
}

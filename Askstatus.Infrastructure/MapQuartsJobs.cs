using Askstatus.Infrastructure.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Askstatus.Infrastructure;

public static class MapQuartsJobs
{
    public static async Task MapJobs(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var runPublishToMqttJobEverySeconds = configuration.GetValue<int>("AskstatusSettings:RunPublishToMqttJobEverySeconds", 60);
        var schedulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        var publishToMqttJob = JobBuilder.Create<PublishToMqttJob>()
            .WithIdentity(nameof(PublishToMqttJob))
            .Build();

        var publishToMqttTrigger = TriggerBuilder.Create()
            .ForJob(publishToMqttJob)
            .WithSimpleSchedule(schedule =>
                schedule.WithInterval(TimeSpan.FromSeconds(runPublishToMqttJobEverySeconds))
            .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(publishToMqttJob, publishToMqttTrigger);
    }
}

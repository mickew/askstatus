using Testcontainers.Papercut;

namespace Askstatus.Infrastructure.Tests.Common;
public class SMTPServerFixture : IAsyncLifetime
{
    public PapercutContainer PapercutContainer { get; private set; }

    public SMTPServerFixture()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        PapercutContainer = new PapercutBuilder().Build();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public async Task InitializeAsync()
    {
        await PapercutContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await PapercutContainer.DisposeAsync().AsTask();
    }
}

using Testcontainers.Papercut;

namespace Askstatus.Infrastructure.Tests.Common;
public class SMTPServerFixture : IAsyncLifetime
{
    public PapercutContainer PapercutContainer { get; private set; }

    public SMTPServerFixture()
    {
        PapercutContainer = new PapercutBuilder().Build();
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

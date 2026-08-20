using Testcontainers.Papercut;

namespace Askstatus.Infrastructure.Tests.Common;
public class SMTPServerFixture : IAsyncLifetime
{
    public PapercutContainer PapercutContainer { get; private set; }

    public SMTPServerFixture()
    {
        PapercutContainer = new PapercutBuilder("changemakerstudiosus/papercut-smtp:7.0").Build();
    }

    public async ValueTask InitializeAsync()
    {
        await PapercutContainer.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await PapercutContainer.DisposeAsync();
    }
}

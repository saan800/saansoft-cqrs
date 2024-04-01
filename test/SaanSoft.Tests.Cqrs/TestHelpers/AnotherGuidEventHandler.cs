using SaanSoft.Cqrs.Handler;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class AnotherGuidEventHandler : IEventHandler<GuidEvent>
{
    public Task HandleAsync(GuidEvent evt, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

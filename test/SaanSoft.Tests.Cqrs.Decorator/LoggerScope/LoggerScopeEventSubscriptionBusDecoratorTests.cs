using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeEventSubscriptionBusTests : EventSubscriptionBusTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override IEventSubscriptionBus SutSubscriptionBus =>
        new LoggerScopeEventSubscriptionBus(_logger, InMemoryEventBus);

    [Fact]
    public async Task RunAsync_calls_BeginScope()
    {
        await SutSubscriptionBus.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

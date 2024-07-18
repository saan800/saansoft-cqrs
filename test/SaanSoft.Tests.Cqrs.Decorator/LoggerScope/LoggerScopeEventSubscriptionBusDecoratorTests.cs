using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeEventSubscriptionBusDecoratorTests : EventSubscriptionBusDecoratorTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override IEventSubscriptionBusDecorator SutSubscriptionBusDecorator =>
        new LoggerScopeEventSubscriptionBusDecorator(_logger, InMemoryEventBus);

    [Fact]
    public async Task RunAsync_calls_BeginScope()
    {
        await SutSubscriptionBusDecorator.RunAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

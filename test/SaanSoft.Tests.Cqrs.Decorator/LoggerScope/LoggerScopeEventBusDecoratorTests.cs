using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeEventBusDecoratorTests : EventBusDecoratorTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override IEventBusDecorator SutPublisherDecorator =>
        new LoggerScopeEventBusDecorator(_logger, InMemoryEventBus);

    [Fact]
    public async Task QueueAsync_calls_BeginScope()
    {
        await SutPublisherDecorator.QueueAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueManyAsync_calls_BeginScope_once_per_event()
    {
        await SutPublisherDecorator.QueueManyAsync([
            new MyEvent(Guid.NewGuid()),
            new MyEvent(Guid.NewGuid())
        ]);

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappenedTwiceExactly();
    }
}

using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeQuerySubscriptionBusDecoratorTests : QuerySubscriptionBusDecoratorTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override IQuerySubscriptionBus SutSubscriptionBusDecorator =>
        new LoggerScopeQuerySubscriptionBusDecorator(_logger, InMemoryQueryBus);

    [Fact]
    public async Task RunAsync_calls_BeginScope()
    {
        await SutSubscriptionBusDecorator.RunAsync(new MyQuery());

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

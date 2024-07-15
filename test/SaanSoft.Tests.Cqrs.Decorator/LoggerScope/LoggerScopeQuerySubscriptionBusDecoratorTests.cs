using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeQuerySubscriptionBusTests : QuerySubscriptionBusTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override IQuerySubscriptionBus SutSubscriptionBus =>
        new LoggerScopeQuerySubscriptionBus(_logger, InMemoryQueryBus);

    [Fact]
    public async Task RunAsync_calls_BeginScope()
    {
        await SutSubscriptionBus.RunAsync(new MyQuery());

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

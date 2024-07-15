using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeCommandSubscriptionBusTests : CommandSubscriptionBusTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override ICommandSubscriptionBus SutSubscriptionBus =>
        new LoggerScopeCommandSubscriptionBus(_logger, InMemoryCommandBus);

    [Fact]
    public async Task RunAsync_calls_BeginScope()
    {
        await SutSubscriptionBus.RunAsync(new MyCommand());

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }

    [Fact]
    public async Task RunAsyncWithResponse_calls_BeginScope()
    {
        await SutSubscriptionBus.RunAsync(new MyCommandWithResponse { Message = "hi" });

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

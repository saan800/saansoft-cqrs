using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeCommandSubscriptionBusDecoratorTests : CommandSubscriptionBusDecoratorTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override ICommandSubscriptionBus SutSubscriptionBusDecorator =>
        new LoggerScopeCommandSubscriptionBusDecorator(_logger, InMemoryCommandBus);

    [Fact]
    public async Task RunAsync_calls_BeginScope()
    {
        await SutSubscriptionBusDecorator.RunAsync(new MyCommand());

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }

    [Fact]
    public async Task RunAsyncWithResponse_calls_BeginScope()
    {
        await SutSubscriptionBusDecorator.RunAsync(new MyCommandWithResponse { Message = "hi" });

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeCommandBusTests : CommandBusTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override ICommandBus SutPublisherDecorator =>
        new LoggerScopeCommandBus(_logger, InMemoryCommandBus);

    [Fact]
    public async Task ExecuteAsync_calls_BeginScope()
    {
        await SutPublisherDecorator.ExecuteAsync(new MyCommand());

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsyncWithResponse_calls_BeginScope()
    {
        await SutPublisherDecorator.ExecuteAsync(new MyCommandWithResponse { Message = "hi" });

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_calls_BeginScope()
    {
        await SutPublisherDecorator.QueueAsync(new MyCommand());

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

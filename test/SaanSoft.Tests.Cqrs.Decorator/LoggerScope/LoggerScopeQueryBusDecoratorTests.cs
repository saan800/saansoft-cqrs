using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

namespace SaanSoft.Tests.Cqrs.Decorator.LoggerScope;

public class LoggerScopeQueryBusTests : QueryBusTestSetup
{
    private readonly ILogger _logger = A.Fake<ILogger>();

    protected override IQueryBus SutPublisherDecorator =>
        new LoggerScopeQueryBus(_logger, InMemoryQueryBus);

    [Fact]
    public async Task FetchAsync_calls_BeginScope()
    {
        await SutPublisherDecorator.FetchAsync(new MyQuery());

        A.CallTo(() => _logger.BeginScope(A<Dictionary<string, object>>._)).MustHaveHappened();
    }
}

using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryQueryBusTests
{
    private readonly ILogger _logger;
    private readonly QueryBusOptions _options;

    public InMemoryQueryBusTests()
    {
        _options = new QueryBusOptions { LogLevel = LogLevel.Information };
        _logger = A.Fake<ILogger>();
        A.CallTo(() => _logger.IsEnabled(A<LogLevel>.Ignored)).Returns(true);
    }

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new InMemoryQueryBus(null, _logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        var serviceCollection = new ServiceCollection();

        Action act = () => new InMemoryQueryBus(serviceCollection.BuildServiceProvider(), null);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "logger");
    }

    [Fact]
    public async Task QueryAsync_handler_exists_in_serviceProvider()
    {
        var handler = A.Fake<IQueryHandler<MyQuery, QueryResponse>>();
        A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new QueryResponse());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler);

        var sut = new InMemoryQueryBus(serviceCollection.BuildServiceProvider(), _logger, _options);
        var result = await sut.QueryAsync(new MyQuery());
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappened();
    }

    [Fact]
    public async Task QueryAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new InMemoryQueryBus(serviceCollection.BuildServiceProvider(), _logger);

        await sut.Invoking(y => y.QueryAsync(new MyQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("No service for type") &&
                x.Message.EndsWith("has been registered.")
            );
    }

    [Fact]
    public async Task QueryAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error()
    {
        var handler1 = A.Fake<IQueryHandler<MyQuery, QueryResponse>>();
        var handler2 = A.Fake<IQueryHandler<MyQuery, QueryResponse>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler1);
        serviceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler2);

        var sut = new InMemoryQueryBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.Invoking(y => y.QueryAsync(new MyQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("Only one service for type") &&
                x.Message.Contains("can be registered")
            );

        A.CallTo(() => handler1.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => handler2.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }
}

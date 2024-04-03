using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class LocalQueryBusTests
{
    private readonly ILogger _logger = A.Fake<ILogger>();
    private readonly QueryBusOptions _options = new();

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new LocalQueryBus(null, _logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        var serviceCollection = new ServiceCollection();

        Action act = () => new LocalQueryBus(serviceCollection.BuildServiceProvider(), null);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "logger");
    }

    [Fact]
    public async Task QueryAsync_handler_exists_in_serviceProvider()
    {
        var handler = A.Fake<IQueryHandler<GuidQuery, QueryResult>>();
        A.CallTo(() => handler.HandleAsync(A<GuidQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new QueryResult());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IQueryHandler<GuidQuery, QueryResult>>(_ => handler);

        var sut = new LocalQueryBus(serviceCollection.BuildServiceProvider(), _logger, _options);
        var result = await sut.QueryAsync<GuidQuery, QueryResult>(new GuidQuery());
        result.IsSuccess.Should().BeTrue();

        A.CallTo(() => handler.HandleAsync(A<GuidQuery>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappened();
    }

    [Fact]
    public async Task QueryAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalQueryBus(serviceCollection.BuildServiceProvider(), _logger);

        await sut.Invoking(y => y.QueryAsync<GuidQuery, QueryResult>(new GuidQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("No service for type") &&
                x.Message.EndsWith("has been registered.")
            );
    }

    [Fact]
    public async Task QueryAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error()
    {
        var handler1 = A.Fake<IQueryHandler<GuidQuery, QueryResult>>();
        var handler2 = A.Fake<IQueryHandler<GuidQuery, QueryResult>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IQueryHandler<GuidQuery, QueryResult>>(_ => handler1);
        serviceCollection.AddScoped<IQueryHandler<GuidQuery, QueryResult>>(_ => handler2);

        var sut = new LocalQueryBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.Invoking(y => y.QueryAsync<GuidQuery, QueryResult>(new GuidQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("Only one service for type") &&
                x.Message.Contains("can be registered")
            );

        A.CallTo(() => handler1.HandleAsync(A<GuidQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => handler2.HandleAsync(A<GuidQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }
}

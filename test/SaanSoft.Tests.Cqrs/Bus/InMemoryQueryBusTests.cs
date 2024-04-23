using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryQueryBusTests : TestSetup
{
    private readonly QueryBusOptions _options = new() { LogLevel = LogLevel.Information };

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new InMemoryQueryBus(null, Logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        Action act = () => new InMemoryQueryBus(GetServiceProvider(), null);

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

        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler);

        var sut = new InMemoryQueryBus(GetServiceProvider(), Logger, _options);
        var result = await sut.QueryAsync(new MyQuery());
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappened();
    }

    [Fact]
    public async Task QueryAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var sut = new InMemoryQueryBus(GetServiceProvider(), Logger);

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

        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler1);
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, QueryResponse>>(_ => handler2);

        var sut = new InMemoryQueryBus(GetServiceProvider(), Logger);
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

using AutoFixture.Xunit2;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryQueryBusTests : TestSetup
{
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

    [Theory]
    [InlineAutoData]
    public async Task FetchAsync_handler_exists_in_serviceProvider(string data)
    {
        var handler = A.Fake<IQueryHandler<MyQuery, MyQueryResponse>>();
        A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new MyQueryResponse(data));

        ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>>(_ => handler);

        var sut = new InMemoryQueryBus(GetServiceProvider(), Logger);
        var result = await sut.FetchAsync(new MyQuery());
        result.Should().NotBeNull();
        result.SomeData.Should().Be(data);

        A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappened();
    }

    [Fact]
    public async Task FetchAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var sut = new InMemoryQueryBus(GetServiceProvider(), Logger);

        await sut.Invoking(y => y.FetchAsync(new MyQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("No service for type") &&
                x.Message.EndsWith("has been registered.")
            );
    }

    [Fact]
    public async Task FetchAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error()
    {
        var handler1 = A.Fake<IQueryHandler<MyQuery, MyQueryResponse>>();
        var handler2 = A.Fake<IQueryHandler<MyQuery, MyQueryResponse>>();

        ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>>(_ => handler1);
        ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>>(_ => handler2);

        var sut = new InMemoryQueryBus(GetServiceProvider(), Logger);
        await sut.Invoking(y => y.FetchAsync(new MyQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("Only one service for type") &&
                x.Message.Contains("can be registered")
            );

        A.CallTo(() => handler1.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => handler2.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }
}

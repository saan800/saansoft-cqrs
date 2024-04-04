using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryEventBusTests : TestSetup
{
    private readonly EventBusOptions _options = new() { LogLevel = LogLevel.Information };

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new InMemoryEventBus(null, Logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        Action act = () => new InMemoryEventBus(GetServiceProvider(), null);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "logger");
    }

    [Fact]
    public async Task ExecuteAsync_single_handler_exists_in_serviceProvider()
    {
        var eventHandler = A.Fake<IEventHandler<MyEvent>>();

        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => eventHandler);

        var sut = new InMemoryEventBus(GetServiceProvider(), Logger, _options);
        await sut.QueueAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => eventHandler.HandleAsync(A<MyEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    /// <summary>
    /// Unlike commands and queries, having multiple event handlers is fine,
    /// and both event handlers should be run
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider()
    {
        var eventHandler = A.Fake<IEventHandler<MyEvent>>();
        var anotherEventHandler = A.Fake<IEventHandler<MyEvent>>();

        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => eventHandler);
        ServiceCollection.AddScoped<IEventHandler<MyEvent>>(_ => anotherEventHandler);

        var sut = new InMemoryEventBus(GetServiceProvider(), Logger);
        await sut.QueueAsync(new MyEvent(Guid.NewGuid()));

        A.CallTo(() => eventHandler.HandleAsync(A<MyEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => anotherEventHandler.HandleAsync(A<MyEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    /// <summary>
    /// Unlike commands and queries, having no event handlers is fine, it just does nothing.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_do_nothing()
    {
        var sut = new InMemoryEventBus(GetServiceProvider(), Logger);
        await sut.QueueAsync(new MyEvent(Guid.NewGuid()));

        Assert.True(true);
    }
}

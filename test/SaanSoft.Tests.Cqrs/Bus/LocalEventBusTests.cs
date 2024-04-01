using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class LocalEventBusTests
{
    [Fact]
    public async Task ExecuteAsync_single_handler_exists_in_serviceProvider()
    {
        var eventHandler = A.Fake<IEventHandler<GuidEvent>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IEventHandler<GuidEvent>>(_ => eventHandler);

        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));

        A.CallTo(() => eventHandler.HandleAsync(A<GuidEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider()
    {
        var eventHandler = A.Fake<IEventHandler<GuidEvent>>();
        var anotherEventHandler = A.Fake<IEventHandler<GuidEvent>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IEventHandler<GuidEvent>>(_ => eventHandler);
        serviceCollection.AddScoped<IEventHandler<GuidEvent>>(_ => anotherEventHandler);

        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));

        A.CallTo(() => eventHandler.HandleAsync(A<GuidEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => anotherEventHandler.HandleAsync(A<GuidEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_do_nothing()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));

        Assert.True(true);
    }
}

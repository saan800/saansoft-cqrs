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
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IEventHandler<GuidEvent>, GuidEventHandler>();

        // TODO: FakeItEasy so can check that each handler was actually called
        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider()
    {
        var serviceCollection = new ServiceCollection();
        // TODO: add all from assembly
        serviceCollection.AddScoped<IEventHandler<GuidEvent>, GuidEventHandler>();
        serviceCollection.AddScoped<IEventHandler<GuidEvent>, AnotherGuidEventHandler>();

        // TODO: FakeItEasy so can check that each handler was actually called
        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_do_nothing()
    {
        var serviceCollection = new ServiceCollection();

        // TODO: FakeItEasy so can check that each handler was actually called
        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));
        Assert.True(true);
    }
}

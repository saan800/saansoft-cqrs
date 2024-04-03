using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class LocalEventBusTests
{
    private readonly ILogger _logger = A.Fake<ILogger>();
    private readonly EventBusOptions _options = new();

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new LocalEventBus(null, _logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        var serviceCollection = new ServiceCollection();

        Action act = () => new LocalEventBus(serviceCollection.BuildServiceProvider(), null);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "logger");
    }

    [Fact]
    public async Task ExecuteAsync_single_handler_exists_in_serviceProvider()
    {
        var eventHandler = A.Fake<IEventHandler<GuidEvent>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IEventHandler<GuidEvent>>(_ => eventHandler);

        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider(), _logger, _options);
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));

        A.CallTo(() => eventHandler.HandleAsync(A<GuidEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    /// <summary>
    /// Unlike commands and queries, having multiple event handlers is fine,
    /// and both event handlers should be run
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider()
    {
        var eventHandler = A.Fake<IEventHandler<GuidEvent>>();
        var anotherEventHandler = A.Fake<IEventHandler<GuidEvent>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IEventHandler<GuidEvent>>(_ => eventHandler);
        serviceCollection.AddScoped<IEventHandler<GuidEvent>>(_ => anotherEventHandler);

        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));

        A.CallTo(() => eventHandler.HandleAsync(A<GuidEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => anotherEventHandler.HandleAsync(A<GuidEvent>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    /// <summary>
    /// Unlike commands and queries, having no event handlers is fine, it just does nothing.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_do_nothing()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalEventBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.QueueAsync(new GuidEvent(Guid.NewGuid()));

        Assert.True(true);
    }
}

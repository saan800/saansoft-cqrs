using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryEventBusTests
{
    private readonly ILogger _logger;
    private readonly EventBusOptions _options;

    public InMemoryEventBusTests()
    {
        _options = new EventBusOptions { LogLevel = LogLevel.Information };
        _logger = A.Fake<ILogger>();
        A.CallTo(() => _logger.IsEnabled(A<LogLevel>.Ignored)).Returns(true);
    }

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new InMemoryEventBus(null, _logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        var serviceCollection = new ServiceCollection();

        Action act = () => new InMemoryEventBus(serviceCollection.BuildServiceProvider(), null);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "logger");
    }

    [Fact]
    public async Task ExecuteAsync_single_handler_exists_in_serviceProvider()
    {
        var eventHandler = A.Fake<IEventHandler<MyEvent>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IEventHandler<MyEvent>>(_ => eventHandler);

        var sut = new InMemoryEventBus(serviceCollection.BuildServiceProvider(), _logger, _options);
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

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IEventHandler<MyEvent>>(_ => eventHandler);
        serviceCollection.AddScoped<IEventHandler<MyEvent>>(_ => anotherEventHandler);

        var sut = new InMemoryEventBus(serviceCollection.BuildServiceProvider(), _logger);
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
        var serviceCollection = new ServiceCollection();

        var sut = new InMemoryEventBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.QueueAsync(new MyEvent(Guid.NewGuid()));

        Assert.True(true);
    }
}

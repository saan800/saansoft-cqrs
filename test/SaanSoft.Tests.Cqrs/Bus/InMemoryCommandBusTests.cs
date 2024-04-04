using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryCommandBusTests
{
    private readonly ILogger _logger;
    private readonly CommandBusOptions _options;

    public InMemoryCommandBusTests()
    {
        _options = new CommandBusOptions { LogLevel = LogLevel.Information };
        _logger = A.Fake<ILogger>();
        A.CallTo(() => _logger.IsEnabled(A<LogLevel>.Ignored)).Returns(true);
    }

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new InMemoryCommandBus(null, _logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        var serviceCollection = new ServiceCollection();

        Action act = () => new InMemoryCommandBus(serviceCollection.BuildServiceProvider(), null);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "logger");
    }

    [Fact]
    public async Task ExecuteAsync_handler_exists_in_serviceProvider()
    {
        var handler = A.Fake<ICommandHandler<MyCommand>>();
        A.CallTo(() => handler.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new CommandResponse());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler);

        var sut = new InMemoryCommandBus(serviceCollection.BuildServiceProvider(), _logger, _options);
        var result = await sut.ExecuteAsync(new MyCommand());
        result.IsSuccess.Should().BeTrue();

        A.CallTo(() => handler.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new InMemoryCommandBus(serviceCollection.BuildServiceProvider(), _logger);

        await sut.Invoking(y => y.ExecuteAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("No service for type") &&
                x.Message.EndsWith("has been registered.")
            );
    }

    [Fact]
    public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error()
    {
        var handler1 = A.Fake<ICommandHandler<MyCommand>>();
        var handler2 = A.Fake<ICommandHandler<MyCommand>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
        serviceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler2);

        var sut = new InMemoryCommandBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.Invoking(y => y.ExecuteAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("Only one service for type") &&
                x.Message.Contains("can be registered")
            );

        A.CallTo(() => handler1.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => handler2.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_handler_exists_in_serviceProvider()
    {
        var handler = A.Fake<ICommandHandler<MyCommand>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler);

        var sut = new InMemoryCommandBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.QueueAsync(new MyCommand());

        A.CallTo(() => handler.HandleAsync(A<MyCommand>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new InMemoryCommandBus(serviceCollection.BuildServiceProvider(), _logger);

        await sut.Invoking(y => y.ExecuteAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("No service for type") &&
                x.Message.EndsWith("has been registered.")
            );
    }

    [Fact]
    public async Task QueueAsync_multiple_handlers_in_serviceProvider_should_throw_error()
    {
        var handler1 = A.Fake<ICommandHandler<MyCommand>>();
        var handler2 = A.Fake<ICommandHandler<MyCommand>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
        serviceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler2);

        var sut = new InMemoryCommandBus(serviceCollection.BuildServiceProvider(), _logger);
        await sut.Invoking(y => y.QueueAsync(new MyCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("Only one service for type") &&
                x.Message.Contains("can be registered")
            );

        A.CallTo(() => handler1.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => handler2.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }
}

using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryCommandBusTests : TestSetup
{
    private readonly CommandBusOptions _options = new() { LogLevel = LogLevel.Information };

    [Fact]
    public void Cant_create_with_null_serviceProvider()
    {
        Action act = () => new InMemoryCommandBus(null, Logger);

        act.Should()
            .Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "serviceProvider");
    }

    [Fact]
    public void Cant_create_with_null_logger()
    {
        Action act = () => new InMemoryCommandBus(GetServiceProvider(), null);

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

        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler);

        var sut = new InMemoryCommandBus(GetServiceProvider(), Logger, _options);
        var result = await sut.ExecuteAsync(new MyCommand());
        result.IsSuccess.Should().BeTrue();

        A.CallTo(() => handler.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var sut = new InMemoryCommandBus(GetServiceProvider(), Logger);

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

        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler2);

        var sut = new InMemoryCommandBus(GetServiceProvider(), Logger);
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

        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler);

        var sut = new InMemoryCommandBus(GetServiceProvider(), Logger);
        await sut.QueueAsync(new MyCommand());

        A.CallTo(() => handler.HandleAsync(A<MyCommand>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var sut = new InMemoryCommandBus(GetServiceProvider(), Logger);

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

        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
        ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler2);

        var sut = new InMemoryCommandBus(GetServiceProvider(), Logger);
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

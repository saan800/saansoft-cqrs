using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class LocalCommandBusTests
{
    [Fact]
    public async Task ExecuteAsync_handler_exists_in_serviceProvider()
    {
        var handler = A.Fake<ICommandHandler<GuidCommand>>();
        A.CallTo(() => handler.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new CommandResult());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>>(_ => handler);

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());
        var result = await sut.ExecuteAsync(new GuidCommand());
        result.IsSuccess.Should().BeTrue();

        A.CallTo(() => handler.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());

        await sut.Invoking(y => y.ExecuteAsync(new GuidCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("No service for type") &&
                x.Message.EndsWith("has been registered")
            );
    }

    [Fact]
    public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error()
    {
        var handler1 = A.Fake<ICommandHandler<GuidCommand>>();
        A.CallTo(() => handler1.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new CommandResult());

        var handler2 = A.Fake<ICommandHandler<GuidCommand>>();
        A.CallTo(() => handler2.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new CommandResult());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>>(_ => handler1);
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>>(_ => handler2);

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());
        await sut.Invoking(y => y.ExecuteAsync(new GuidCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("Only one service for type") &&
                x.Message.Contains("can be registered")
            );

        A.CallTo(() => handler1.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => handler2.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_handler_exists_in_serviceProvider()
    {
        var handler = A.Fake<ICommandHandler<GuidCommand>>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>>(_ => handler);

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidCommand());

        A.CallTo(() => handler.HandleAsync(A<GuidCommand>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueueAsync_no_handler_in_serviceProvider_should_throw_error()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());

        await sut.Invoking(y => y.ExecuteAsync(new GuidCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("No service for type") &&
                x.Message.EndsWith("has been registered")
            );
    }

    [Fact]
    public async Task QueueAsync_multiple_handlers_in_serviceProvider_should_throw_error()
    {
        var handler1 = A.Fake<ICommandHandler<GuidCommand>>();
        A.CallTo(() => handler1.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new CommandResult());

        var handler2 = A.Fake<ICommandHandler<GuidCommand>>();
        A.CallTo(() => handler2.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new CommandResult());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>>(_ => handler1);
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>>(_ => handler2);

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());
        await sut.Invoking(y => y.QueueAsync(new GuidCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .Where(x =>
                x.Message.StartsWith("Only one service for type") &&
                x.Message.Contains("can be registered")
            );

        A.CallTo(() => handler1.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => handler2.HandleAsync(A<GuidCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }
}

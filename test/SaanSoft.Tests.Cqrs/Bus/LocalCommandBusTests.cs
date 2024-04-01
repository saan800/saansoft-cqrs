using Microsoft.Extensions.DependencyInjection;

using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class LocalCommandBusTests
{
    [Fact]
    public async Task ExecuteAsync_handler_exists_in_serviceProvider()
    {
        var serviceCollection = new ServiceCollection();
        // TODO: add all from assembly
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>, GuidCommandHandler>();

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());
        var result = await sut.ExecuteAsync(new GuidCommand());
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_no_handler_in_serviceProvider_should_throw_exception()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());

        await sut.Invoking(y => y.ExecuteAsync(new GuidCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"No service for type '{typeof(ICommandHandler<GuidCommand>)}' has been registered.");
    }

    [Fact]
    public async Task QueueAsync_handler_exists_in_serviceProvider()
    {
        var serviceCollection = new ServiceCollection();
        // TODO: add all from assembly
        serviceCollection.AddScoped<ICommandHandler<GuidCommand>, GuidCommandHandler>();

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());
        await sut.QueueAsync(new GuidCommand());
        Assert.True(true);
    }

    [Fact]
    public async Task QueueAsync_no_handler_in_serviceProvider_should_throw_exception()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalCommandBus(serviceCollection.BuildServiceProvider());

        await sut.Invoking(y => y.QueueAsync(new GuidCommand()))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"No service for type '{typeof(ICommandHandler<GuidCommand>)}' has been registered.");
    }
}

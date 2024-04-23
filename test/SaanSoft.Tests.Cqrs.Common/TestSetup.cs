using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Tests.Cqrs.Common;

public abstract class TestSetup
{
    protected readonly ILogger Logger;
    protected readonly ServiceCollection ServiceCollection;

    protected TestSetup()
    {
        Logger = A.Fake<ILogger>();
        A.CallTo(() => Logger.IsEnabled(A<LogLevel>.Ignored)).Returns(true);

        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<ILogger>(_ => Logger);
        ServiceCollection.AddScoped<ICommandSubscriber<Guid>, InMemoryCommandBus>();
        ServiceCollection.AddScoped<IEventSubscriber<Guid>, InMemoryEventBus>();
        ServiceCollection.AddScoped<IQuerySubscriber<Guid>, InMemoryQueryBus>();
    }

    private IServiceProvider? _serviceProvider = null;
    protected IServiceProvider GetServiceProvider()
        => _serviceProvider ??= ServiceCollection.BuildServiceProvider();
}

using SaanSoft.Cqrs.Decorator.GuidIds.Utilities;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Common;

public abstract class TestSetup
{
    protected readonly ILogger Logger;
    protected readonly ServiceCollection ServiceCollection;

    protected TestSetup()
    {
        Logger = A.Fake<ILogger>();

        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<ILogger>(_ => Logger);

        ServiceCollection.AddScoped<ICommandSubscriptionBus<Guid>, InMemoryCommandBus>();
        ServiceCollection.AddScoped<IEventSubscriptionBus<Guid>, InMemoryEventBus>();
        ServiceCollection.AddScoped<IQuerySubscriptionBus<Guid>, InMemoryQueryBus>();

        ServiceCollection.AddScoped<IIdGenerator<Guid>, GuidIdGenerator>();
    }

    private IServiceProvider? _serviceProvider = null;
    protected IServiceProvider GetServiceProvider()
        => _serviceProvider ??= ServiceCollection.BuildServiceProvider();
}

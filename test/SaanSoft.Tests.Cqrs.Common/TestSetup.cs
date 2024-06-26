using SaanSoft.Cqrs.Decorator.GuidIds.Utilities;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Common;

public abstract class TestSetup
{
    protected readonly ILogger Logger;
    protected readonly IIdGenerator<Guid> IdGenerator;
    protected readonly ServiceCollection ServiceCollection;

    protected TestSetup()
    {
        Logger = A.Fake<ILogger>();
        IdGenerator = new GuidIdGenerator();

        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<ILogger>(_ => Logger);
        ServiceCollection.AddScoped<IIdGenerator<Guid>>(_ => IdGenerator);

        ServiceCollection.AddScoped<ICommandSubscriptionBus<Guid>, InMemoryCommandBus>();
        ServiceCollection.AddScoped<IEventSubscriptionBus<Guid>, InMemoryEventBus>();
        ServiceCollection.AddScoped<IQuerySubscriptionBus<Guid>, InMemoryQueryBus>();

        ServiceCollection.AddScoped<IIdGenerator<Guid>, GuidIdGenerator>();
    }

    private IServiceProvider? _serviceProvider = null;
    protected IServiceProvider GetServiceProvider()
        => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

    protected InMemoryCommandBus InMemoryCommandBus => new(GetServiceProvider(), IdGenerator, Logger);

    protected InMemoryEventBus InMemoryEventBus => new(GetServiceProvider(), IdGenerator, Logger);

    protected InMemoryQueryBus InMemoryQueryBus => new(GetServiceProvider(), IdGenerator, Logger);
}

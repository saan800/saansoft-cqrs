using MongoDB.Bson;
using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Root;

public class ServiceCollectionExtensionTests : TestSetup
{
    private readonly ServiceCollection _serviceCollection;

    public ServiceCollectionExtensionTests()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddScoped<IMongoDatabase>(_ => Database);
    }

    [Fact]
    public void AddGuidStores()
    {
        _serviceCollection.AddGuidStores();
        var serviceProvider = _serviceCollection.BuildServiceProvider();

        var commandStore = serviceProvider.GetRequiredService<ICommandStore<Guid>>();
        commandStore.Should().BeOfType<CommandStore>();
        commandStore.Should().BeAssignableTo<CommandStore<Guid>>();

        var eventStore = serviceProvider.GetRequiredService<IEventStore<Guid, Guid>>();
        eventStore.Should().BeOfType<EventStore>();
        eventStore.Should().BeAssignableTo<EventStore<Guid, Guid>>();

        var queryStore = serviceProvider.GetRequiredService<IQueryStore<Guid>>();
        queryStore.Should().BeOfType<QueryStore>();
        queryStore.Should().BeAssignableTo<QueryStore<Guid>>();
    }

    [Fact]
    public void AddGuidStores_via_generic_ObjectId_TEntityKey()
    {
        _serviceCollection.AddGuidStores<ObjectId>();
        var serviceProvider = _serviceCollection.BuildServiceProvider();

        var commandStore = serviceProvider.GetRequiredService<ICommandStore<Guid>>();
        commandStore.Should().BeOfType<CommandStore>();
        commandStore.Should().BeAssignableTo<CommandStore<Guid>>();

        var eventStore = serviceProvider.GetRequiredService<IEventStore<Guid, ObjectId>>();
        eventStore.Should().BeAssignableTo<EventStore<ObjectId>>();
        eventStore.Should().BeAssignableTo<EventStore<Guid, ObjectId>>();

        var queryStore = serviceProvider.GetRequiredService<IQueryStore<Guid>>();
        queryStore.Should().BeOfType<QueryStore>();
        queryStore.Should().BeAssignableTo<QueryStore<Guid>>();
    }

    [Fact]
    public void AddGuidStores_via_generic_Guid_TEntityKey()
    {
        _serviceCollection.AddGuidStores<Guid>();
        var serviceProvider = _serviceCollection.BuildServiceProvider();

        var commandStore = serviceProvider.GetRequiredService<ICommandStore<Guid>>();
        commandStore.Should().BeOfType<CommandStore>();
        commandStore.Should().BeAssignableTo<CommandStore<Guid>>();

        var eventStore = serviceProvider.GetRequiredService<IEventStore<Guid, Guid>>();
        eventStore.Should().BeAssignableTo<EventStore<Guid>>();
        eventStore.Should().BeAssignableTo<EventStore<Guid, Guid>>();

        var queryStore = serviceProvider.GetRequiredService<IQueryStore<Guid>>();
        queryStore.Should().BeOfType<QueryStore>();
        queryStore.Should().BeAssignableTo<QueryStore<Guid>>();
    }
}

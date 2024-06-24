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

        var commandStore = serviceProvider.GetRequiredService<ICommandRepository<Guid>>();
        commandStore.Should().BeOfType<CommandRepository>();
        commandStore.Should().BeAssignableTo<CommandRepository<Guid>>();

        var eventStore = serviceProvider.GetRequiredService<IEventRepository<Guid, Guid>>();
        eventStore.Should().BeOfType<EventRepository>();
        eventStore.Should().BeAssignableTo<EventRepository<Guid, Guid>>();

        var queryStore = serviceProvider.GetRequiredService<IQueryRepository<Guid>>();
        queryStore.Should().BeOfType<QueryRepository>();
        queryStore.Should().BeAssignableTo<QueryRepository<Guid>>();
    }

    [Fact]
    public void AddGuidStores_via_generic_ObjectId_TEntityKey()
    {
        _serviceCollection.AddGuidStores<ObjectId>();
        var serviceProvider = _serviceCollection.BuildServiceProvider();

        var commandStore = serviceProvider.GetRequiredService<ICommandRepository<Guid>>();
        commandStore.Should().BeOfType<CommandRepository>();
        commandStore.Should().BeAssignableTo<CommandRepository<Guid>>();

        var eventStore = serviceProvider.GetRequiredService<IEventRepository<Guid, ObjectId>>();
        eventStore.Should().BeAssignableTo<EventRepository<ObjectId>>();
        eventStore.Should().BeAssignableTo<EventRepository<Guid, ObjectId>>();

        var queryStore = serviceProvider.GetRequiredService<IQueryRepository<Guid>>();
        queryStore.Should().BeOfType<QueryRepository>();
        queryStore.Should().BeAssignableTo<QueryRepository<Guid>>();
    }

    [Fact]
    public void AddGuidStores_via_generic_Guid_TEntityKey()
    {
        _serviceCollection.AddGuidStores<Guid>();
        var serviceProvider = _serviceCollection.BuildServiceProvider();

        var commandStore = serviceProvider.GetRequiredService<ICommandRepository<Guid>>();
        commandStore.Should().BeOfType<CommandRepository>();
        commandStore.Should().BeAssignableTo<CommandRepository<Guid>>();

        var eventStore = serviceProvider.GetRequiredService<IEventRepository<Guid, Guid>>();
        eventStore.Should().BeAssignableTo<EventRepository<Guid>>();
        eventStore.Should().BeAssignableTo<EventRepository<Guid, Guid>>();

        var queryStore = serviceProvider.GetRequiredService<IQueryRepository<Guid>>();
        queryStore.Should().BeOfType<QueryRepository>();
        queryStore.Should().BeAssignableTo<QueryRepository<Guid>>();
    }
}

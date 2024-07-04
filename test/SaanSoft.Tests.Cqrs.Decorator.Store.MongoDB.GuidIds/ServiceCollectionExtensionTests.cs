using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Decorator.Store;
using SaanSoft.Cqrs.Decorator.Store.MongoDB;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds;

public abstract class ServiceCollectionExtensionTests : TestSetup
{
    private readonly ServiceCollection _serviceCollection;

    protected ServiceCollectionExtensionTests()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddScoped<ILogger>(_ => Logger);
        _serviceCollection.AddScoped<IMongoDatabase>(_ => Database);
        _serviceCollection.AddScoped<IIdGenerator>(_ => IdGenerator);
    }

    public class CommandRepositoryExtensions : ServiceCollectionExtensionTests
    {
        [Fact]
        public void Should_register_ICommandRepository_interfaces()
        {
            _serviceCollection.AddCommandRepository();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<ICommandRepository>();
            repository1.Should().BeOfType<CommandRepository>();
            repository1.Should().BeAssignableTo<CommandRepository>();
            repository1.Should().BeAssignableTo<CommandRepository<Guid>>();
            repository1.Should().BeAssignableTo<ICommandRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<ICommandRepository<Guid>>();
            repository2.Should().BeOfType<CommandRepository>();
            repository2.Should().BeAssignableTo<CommandRepository>();
            repository2.Should().BeAssignableTo<CommandRepository<Guid>>();
            repository2.Should().BeAssignableTo<ICommandRepository>();
        }

        [Fact]
        public void Should_register_ICommandHandlerRepository_interfaces()
        {
            _serviceCollection.AddCommandRepository();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<ICommandHandlerRepository>();
            repository1.Should().BeOfType<CommandRepository>();
            repository1.Should().BeAssignableTo<CommandRepository>();
            repository1.Should().BeAssignableTo<CommandRepository<Guid>>();
            repository1.Should().BeAssignableTo<ICommandHandlerRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<ICommandHandlerRepository<Guid>>();
            repository2.Should().BeOfType<CommandRepository>();
            repository2.Should().BeAssignableTo<CommandRepository>();
            repository2.Should().BeAssignableTo<CommandRepository<Guid>>();
            repository2.Should().BeAssignableTo<ICommandHandlerRepository>();
        }
    }

    public class EventRepositoryExtensions : ServiceCollectionExtensionTests
    {
        [Fact]
        public void Should_register_IEventRepository_interfaces()
        {
            _serviceCollection.AddEventRepository();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<IEventRepository>();
            repository1.Should().BeOfType<EventRepository>();
            repository1.Should().BeAssignableTo<EventRepository>();
            repository1.Should().BeAssignableTo<EventRepository<Guid>>();
            repository1.Should().BeAssignableTo<EventRepository<Guid, Guid>>();
            repository1.Should().BeAssignableTo<IEventRepository>();
            repository1.Should().BeAssignableTo<IEventRepository<Guid>>();
            repository1.Should().BeAssignableTo<IEventRepository<Guid, Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IEventRepository<Guid>>();
            repository2.Should().BeOfType<EventRepository>();
            repository2.Should().BeAssignableTo<EventRepository>();
            repository2.Should().BeAssignableTo<EventRepository<Guid>>();
            repository2.Should().BeAssignableTo<EventRepository<Guid, Guid>>();
            repository2.Should().BeAssignableTo<IEventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid>>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid, Guid>>();

            var repository3 = serviceProvider.GetRequiredService<IEventRepository<Guid, Guid>>();
            repository3.Should().BeOfType<EventRepository>();
            repository3.Should().BeAssignableTo<EventRepository>();
            repository3.Should().BeAssignableTo<EventRepository<Guid>>();
            repository3.Should().BeAssignableTo<EventRepository<Guid, Guid>>();
            repository2.Should().BeAssignableTo<IEventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid>>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid, Guid>>();
        }

        [Fact]
        public void Should_register_IEventHandlerRepository_interfaces()
        {
            _serviceCollection.AddEventRepository();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<IEventHandlerRepository>();
            repository1.Should().BeOfType<EventRepository>();
            repository1.Should().BeAssignableTo<EventRepository>();
            repository1.Should().BeAssignableTo<EventRepository<Guid>>();
            repository1.Should().BeAssignableTo<EventRepository<Guid, Guid>>();
            repository1.Should().BeAssignableTo<IEventHandlerRepository>();
            repository1.Should().BeAssignableTo<IEventHandlerRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IEventHandlerRepository<Guid>>();
            repository2.Should().BeOfType<EventRepository>();
            repository2.Should().BeAssignableTo<EventRepository>();
            repository2.Should().BeAssignableTo<EventRepository<Guid>>();
            repository2.Should().BeAssignableTo<EventRepository<Guid, Guid>>();
            repository2.Should().BeAssignableTo<IEventHandlerRepository>();
            repository2.Should().BeAssignableTo<IEventHandlerRepository<Guid>>();
        }
    }

    public class EventRepositoryWithTEntityKeyExtensions : ServiceCollectionExtensionTests
    {
        [Fact]
        public void Should_register_IEventRepository_TEntityKey_interfaces()
        {
            _serviceCollection.AddEventRepository<int>();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<IEventRepository<int>>();
            repository1.Should().BeOfType<EventRepository<int>>();
            repository1.Should().BeAssignableTo<EventRepository<int>>();
            repository1.Should().BeAssignableTo<EventRepository<Guid, int>>();
            repository1.Should().BeAssignableTo<IEventRepository<int>>();
            repository1.Should().BeAssignableTo<IEventRepository<Guid, int>>();

            var repository2 = serviceProvider.GetRequiredService<IEventRepository<Guid, int>>();
            repository2.Should().BeOfType<EventRepository<int>>();
            repository2.Should().BeAssignableTo<EventRepository<int>>();
            repository2.Should().BeAssignableTo<EventRepository<Guid, int>>();
            repository2.Should().BeAssignableTo<IEventRepository<int>>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid, int>>();
        }

        [Fact]
        public void Should_register_IEventHandlerRepository_TEntityKey_interfaces()
        {
            _serviceCollection.AddEventRepository<int>();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<IEventHandlerRepository>();
            repository1.Should().BeOfType<EventRepository<int>>();
            repository1.Should().BeAssignableTo<EventRepository<int>>();
            repository1.Should().BeAssignableTo<EventRepository<Guid, int>>();
            repository1.Should().BeAssignableTo<IEventHandlerRepository>();
            repository1.Should().BeAssignableTo<IEventHandlerRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IEventHandlerRepository<Guid>>();
            repository2.Should().BeOfType<EventRepository<int>>();
            repository2.Should().BeAssignableTo<EventRepository<int>>();
            repository2.Should().BeAssignableTo<EventRepository<Guid, int>>();
            repository2.Should().BeAssignableTo<IEventHandlerRepository>();
            repository2.Should().BeAssignableTo<IEventHandlerRepository<Guid>>();
        }
    }

    public class QueryRepositoryExtensions : ServiceCollectionExtensionTests
    {
        [Fact]
        public void AddQueryRepository_should_register_IQueryRepository_interfaces()
        {
            _serviceCollection.AddQueryRepository();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<IQueryRepository>();
            repository1.Should().BeOfType<QueryRepository>();
            repository1.Should().BeAssignableTo<QueryRepository>();
            repository1.Should().BeAssignableTo<QueryRepository<Guid>>();
            repository1.Should().BeAssignableTo<IQueryRepository>();
            repository1.Should().BeAssignableTo<IQueryRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IQueryRepository<Guid>>();
            repository2.Should().BeOfType<QueryRepository>();
            repository2.Should().BeAssignableTo<QueryRepository>();
            repository2.Should().BeAssignableTo<QueryRepository<Guid>>();
            repository2.Should().BeAssignableTo<IQueryRepository>();
            repository2.Should().BeAssignableTo<IQueryRepository<Guid>>();
        }

        [Fact]
        public void AddQueryRepository_should_register_IQueryHandlerRepository_interfaces()
        {
            _serviceCollection.AddQueryRepository();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<IQueryHandlerRepository>();
            repository1.Should().BeOfType<QueryRepository>();
            repository1.Should().BeAssignableTo<QueryRepository>();
            repository1.Should().BeAssignableTo<QueryRepository<Guid>>();
            repository1.Should().BeAssignableTo<IQueryHandlerRepository>();
            repository1.Should().BeAssignableTo<IQueryHandlerRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IQueryHandlerRepository<Guid>>();
            repository2.Should().BeOfType<QueryRepository>();
            repository2.Should().BeAssignableTo<QueryRepository>();
            repository2.Should().BeAssignableTo<QueryRepository<Guid>>();
            repository2.Should().BeAssignableTo<IQueryHandlerRepository>();
            repository2.Should().BeAssignableTo<IQueryHandlerRepository<Guid>>();
        }
    }

    public class AddGuidRepositoriesTests : ServiceCollectionExtensionTests
    {
        [Fact]
        public void Should_register_command_event_and_query_repository_interfaces()
        {
            _serviceCollection.AddGuidRepositories();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<ICommandRepository>();
            repository1.Should().BeOfType<CommandRepository>();
            repository1.Should().BeAssignableTo<ICommandRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IEventRepository>();
            repository2.Should().BeOfType<EventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid>>();

            var repository3 = serviceProvider.GetRequiredService<IQueryRepository>();
            repository3.Should().BeOfType<QueryRepository>();
            repository3.Should().BeAssignableTo<IQueryRepository<Guid>>();
        }

        [Fact]
        public void Should_register_command_event_and_query_HandlerRepository_interfaces()
        {
            _serviceCollection.AddGuidRepositories();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<ICommandHandlerRepository>();
            repository1.Should().BeOfType<CommandRepository>();
            repository1.Should().BeAssignableTo<ICommandHandlerRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IEventHandlerRepository>();
            repository2.Should().BeOfType<EventRepository>();
            repository2.Should().BeAssignableTo<IEventHandlerRepository<Guid>>();

            var repository3 = serviceProvider.GetRequiredService<IQueryHandlerRepository>();
            repository3.Should().BeOfType<QueryRepository>();
            repository3.Should().BeAssignableTo<IQueryHandlerRepository<Guid>>();
        }
    }
}

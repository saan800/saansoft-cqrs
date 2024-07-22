using Microsoft.Extensions.Logging;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB;

public abstract class ServiceCollectionExtensionTests : TestSetup
{
    private readonly ServiceCollection _serviceCollection;

    protected ServiceCollectionExtensionTests()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddScoped<ILogger>(_ => Logger);
        _serviceCollection.AddScoped<IMongoDatabase>(_ => Database);
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
            repository1.Should().BeAssignableTo<CommandRepository>();
            repository1.Should().BeAssignableTo<ICommandRepository>();

            var repository2 = serviceProvider.GetRequiredService<ICommandRepository>();
            repository2.Should().BeOfType<CommandRepository>();
            repository2.Should().BeAssignableTo<CommandRepository>();
            repository2.Should().BeAssignableTo<CommandRepository>();
            repository2.Should().BeAssignableTo<ICommandRepository>();
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
            repository1.Should().BeAssignableTo<EventRepository>();
            repository1.Should().BeAssignableTo<EventRepository<Guid>>();
            repository1.Should().BeAssignableTo<IEventRepository>();
            repository1.Should().BeAssignableTo<IEventRepository>();
            repository1.Should().BeAssignableTo<IEventRepository<Guid>>();

            var repository2 = serviceProvider.GetRequiredService<IEventRepository>();
            repository2.Should().BeOfType<EventRepository>();
            repository2.Should().BeAssignableTo<EventRepository>();
            repository2.Should().BeAssignableTo<EventRepository>();
            repository2.Should().BeAssignableTo<EventRepository<Guid>>();
            repository2.Should().BeAssignableTo<IEventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid>>();

            var repository3 = serviceProvider.GetRequiredService<IEventRepository<Guid>>();
            repository3.Should().BeOfType<EventRepository>();
            repository3.Should().BeAssignableTo<EventRepository>();
            repository3.Should().BeAssignableTo<EventRepository>();
            repository3.Should().BeAssignableTo<EventRepository<Guid>>();
            repository2.Should().BeAssignableTo<IEventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository<Guid>>();
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
            repository1.Should().BeAssignableTo<IEventRepository<int>>();
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
            repository1.Should().BeAssignableTo<IQueryRepository>();
        }
    }

    public class AddGuidRepositoriesTests : ServiceCollectionExtensionTests
    {
        [Fact]
        public void Should_register_command_event_and_query_repository_interfaces()
        {
            _serviceCollection.AddRepositories();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var repository1 = serviceProvider.GetRequiredService<ICommandRepository>();
            repository1.Should().BeOfType<CommandRepository>();
            repository1.Should().BeAssignableTo<ICommandRepository>();

            var repository2 = serviceProvider.GetRequiredService<IEventRepository>();
            repository2.Should().BeOfType<EventRepository>();
            repository2.Should().BeAssignableTo<IEventRepository>();

            var repository3 = serviceProvider.GetRequiredService<IQueryRepository>();
            repository3.Should().BeOfType<QueryRepository>();
            repository3.Should().BeAssignableTo<IQueryRepository>();
        }
    }
}

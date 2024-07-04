using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public abstract class BaseHandlerRepositoryTests<TMessage, TRepository, TCollectionEntity> : TestSetup
     where TMessage : class, IMessage<Guid>
     where TRepository : IMessageHandlerRepository<Guid>, IMessageRepository<Guid, TMessage>
     where TCollectionEntity : class, IMessage<Guid>
{
    protected TRepository SutRepository;
    protected IMongoCollection<TCollectionEntity> MessageCollection;

    protected abstract TMessage CreateNewMessage();

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_should_add_handler_to_record(Type handlerType)
    {
        var message = CreateNewMessage();
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);

        // check the collection that the record exists
        var recordMetadata = await MessageCollection.Find(x => x.Id == message.Id)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync();
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var record = messageHandlers.SingleOrDefault(x => x.TypeFullName == handlerType.FullName);
        record.Should().NotBeNull();
        record.Succeeded.Should().BeTrue();
        record.Exception.Should().BeNull();
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_with_error_should_add_handler_to_record(Type handlerType)
    {
        var message = CreateNewMessage();
        var exception = new Exception("it went wrong");
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);

        // check the collection that the record exists
        var recordMetadata = await MessageCollection.Find(x => x.Id == message.Id)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync();
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var record = messageHandlers.SingleOrDefault(x => x.TypeFullName == handlerType.FullName);

        record.Should().NotBeNull();
        record.Succeeded.Should().BeFalse();
        record.Exception.TypeFullName.Should().Be("System.Exception");
        record.Exception.Message.Should().Be(exception.Message);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_multiple_times_should_only_add_one_record(Type handlerType)
    {
        var message = CreateNewMessage();
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);

        // check the collection that the record exists
        var recordMetadata = await MessageCollection.Find(x => x.Id == message.Id)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync();
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var records = messageHandlers.Where(x => x.TypeFullName == handlerType.FullName).ToList();
        records.Should().NotBeNull();
        records.Count.Should().Be(1);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_with_exception_multiple_times_should_add_multiple_handler_records(Type handlerType)
    {
        var message = CreateNewMessage();
        var exception = new Exception("it went wrong");
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);

        // check the collection that the record exists
        var recordMetadata = await MessageCollection.Find(x => x.Id == message.Id)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync();
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var records = messageHandlers.Where(x => x.TypeFullName == handlerType.FullName).ToList();
        records.Should().NotBeNull();
        records.Count.Should().Be(3);
    }

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_with_exception_then_without_exception_should_only_have_success_record(Type handlerType)
    {
        var message = CreateNewMessage();
        var exception = new Exception("it went wrong");
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType, exception);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);

        // check the collection that the record exists
        var recordMetadata = await MessageCollection.Find(x => x.Id == message.Id)
            .Project(x => x.Metadata)
            .SingleOrDefaultAsync();
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var records = messageHandlers.FirstOrDefault(x => x.TypeFullName == handlerType.FullName);
        records.Should().NotBeNull();
        records.Succeeded.Should().BeTrue();
    }
}


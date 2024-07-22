using SaanSoft.Cqrs.Decorator.Store.Models;
using SaanSoft.Cqrs.Decorator.Store.Utilities;

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public abstract class BaseHandlerRepositoryTests<TMessage, TRepository> : TestSetup
     where TMessage : class, IMessage
     where TRepository : IMessageRepository<TMessage>
{
    protected TRepository SutRepository { get; init; }

    protected abstract TMessage CreateNewMessage();

    #region UpsertHandlerAsync

    [Theory]
    [InlineAutoData]
    public async Task UpsertHandlerAsync_should_add_handler_to_record(Type handlerType)
    {
        var message = CreateNewMessage();
        await SutRepository.InsertAsync(message);
        await SutRepository.UpsertHandlerAsync(message.Id, handlerType);

        // check the collection that the record exists
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
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
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
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
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
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
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
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
        var recordMetadata = (await SutRepository.GetMessageByIdAsync(message.Id))?.Metadata;
        var messageHandlers = recordMetadata.GetValueOrDefaultAs<List<MessageHandler>>(StoreConstants.HandlersKey) ?? [];

        var records = messageHandlers.FirstOrDefault(x => x.TypeFullName == handlerType.FullName);
        records.Should().NotBeNull();
        records.Succeeded.Should().BeTrue();
    }

    #endregion
}


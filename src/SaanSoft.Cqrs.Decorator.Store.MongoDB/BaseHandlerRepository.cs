// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

/// <summary>
/// Base class with common methods for message handler stores
/// You should never directly use BaseHandlerRepository
/// Use <see cref="CommandHandlerRepository{TMessageId}"/>, <see cref="EventHandlerRepository{TMessageId}"/> or <see cref="QueryHandlerRepository{TMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public abstract class BaseHandlerRepository<TMessageId, TMessage> :
    IMessageHandlerRepository<TMessageId, TMessage>,
    IMongoDbRepository
    where TMessageId : struct
    where TMessage : class, IMessage<TMessageId>
{
    protected readonly IMongoDatabase Database;
    protected readonly IIdGenerator<TMessageId> IdGenerator;
    protected readonly ReplaceOptions ReplaceOptions;

    /// <summary>
    /// </summary>
    /// <param name="database"></param>
    /// <param name="idGenerator"></param>
    /// <param name="replaceOptions">IsUpsert is forced to be true, but specify other parameters as desired</param>
    protected BaseHandlerRepository(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null)
    {
        Database = database;
        IdGenerator = idGenerator;

        if (replaceOptions != null)
        {
            replaceOptions.IsUpsert = true;
        }
        ReplaceOptions = replaceOptions ?? new ReplaceOptions { IsUpsert = true };
    }

    public abstract string CollectionName { get; }

    public virtual IMongoCollection<MessageHandlerRecord<TMessageId>> HandlerCollection
        => Database.GetCollection<MessageHandlerRecord<TMessageId>>(CollectionName);

    public virtual async Task UpsertHandlerAsync(IMessage<TMessageId> message, Type handlerType, Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var messageTypeName = messageType.FullName ?? messageType.Name;
        var messageAssembly = messageType.Assembly.GetName().Name ?? messageType.Assembly.GetName().FullName;
        var handlerTypeName = handlerType.FullName ?? handlerType.Name;
        var handlerAssembly = handlerType.Assembly.GetName().Name ?? handlerType.Assembly.GetName().FullName;
        if (string.IsNullOrWhiteSpace(messageTypeName) || string.IsNullOrWhiteSpace(handlerTypeName)) return;

        var record = await HandlerCollection.Find(x =>
                x.MessageTypeName == messageTypeName &&
                x.MessageAssembly == messageAssembly &&
                x.HandlerTypeName == handlerTypeName &&
                x.HandlerAssembly == handlerAssembly
            )
            .FirstOrDefaultAsync(cancellationToken);
        if (record == null)
        {
            record = new MessageHandlerRecord<TMessageId>
            {
                Id = IdGenerator.NewId(),
                MessageTypeName = messageTypeName,
                MessageAssembly = messageAssembly,
                HandlerTypeName = handlerTypeName,
                HandlerAssembly = handlerAssembly,
                CreatedOnUtc = message.MessageOnUtc,
                LastMessageOnUtc = message.MessageOnUtc,
                LastMessageId = message.Id
            };
        }
        else
        {
            record.LastMessageOnUtc = message.MessageOnUtc;
            record.LastMessageId = message.Id;
        }

        if (exception == null)
        {
            record.LastCompletedMessageId = message.Id;
            record.LastFailedMessages = [];
        }
        else
        {
            record.LastFailedMessages.Add(new MessageHandlerRecord<TMessageId>.FailedMessage
            {
                MessageId = message.Id,
                MessageOnUtc = message.MessageOnUtc,
                Exception = new MessageHandlerRecord<TMessageId>.LogException
                {
                    Message = exception.Message,
                    TypeName = exception.GetType().FullName ?? exception.GetType().Name,
                    StackTrace = exception.StackTrace
                }
            });
        }

        await HandlerCollection.ReplaceOneAsync(
            x => x.MessageTypeName == messageTypeName &&
                 x.MessageAssembly == messageAssembly &&
                 x.HandlerTypeName == handlerTypeName &&
                 x.HandlerAssembly == handlerAssembly,
            record,
            ReplaceOptions,
            cancellationToken
        );
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the HandlerCollection
    /// </summary>
    public virtual async Task EnsureCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<MessageHandlerRecord<TMessageId>>.IndexKeys
            .Ascending(x => x.MessageTypeName)
            .Ascending(x => x.MessageAssembly)
            .Ascending(x => x.HandlerTypeName)
            .Ascending(x => x.HandlerAssembly);

        var indexModel =
            new CreateIndexModel<MessageHandlerRecord<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = true, Background = true });

        await HandlerCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }
}

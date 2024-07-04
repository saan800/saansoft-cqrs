// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

/// <summary>
/// Base class with common methods for message stores
/// You should never directly use BaseMessageRepository
/// Use <see cref="CommandRepository{TMessageId}"/>, <see cref="EventRepository{TMessageId,TEntityKey}"/> or <see cref="QueryRepository{TMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public abstract class BaseMessageRepository<TMessageId, TMessage> :
    IMessageRepository<TMessageId, TMessage>,
    IMessageHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
    where TMessage : class, IMessage<TMessageId>
{
    protected readonly IMongoDatabase Database;
    protected readonly IIdGenerator<TMessageId> IdGenerator;
    protected readonly ILogger Logger;
    protected readonly InsertOneOptions InsertOneOptions;

    /// <summary>
    /// </summary>
    /// <param name="database"></param>
    /// <param name="idGenerator"></param>
    /// <param name="logger"></param>
    /// <param name="insertOneOptions"></param>
    protected BaseMessageRepository(
        IMongoDatabase database, IIdGenerator<TMessageId> idGenerator,
        ILogger logger, InsertOneOptions? insertOneOptions = null
        )
    {
        Database = database;
        IdGenerator = idGenerator;
        Logger = logger;
        InsertOneOptions = insertOneOptions ?? new InsertOneOptions();
    }

    public abstract string CollectionName { get; }

    protected virtual IMongoCollection<IMessage<TMessageId>> BaseMessageCollection
        => Database.GetCollection<IMessage<TMessageId>>(CollectionName);

    public virtual async Task InsertAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        if (message.IsReplay) return;

        if (GenericUtils.IsNullOrDefault(message.Id)) message.Id = IdGenerator.NewId();
        await BaseMessageCollection.InsertOneAsync(message, InsertOneOptions, cancellationToken);
    }

    public abstract Task UpsertHandlerAsync(TMessageId id, Type handlerType, Exception? exception = null, CancellationToken cancellationToken = default);

    public abstract Task EnsureCollectionIndexesAsync(CancellationToken cancellationToken = default);
}

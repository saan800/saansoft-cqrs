// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

/// <summary>
/// Base class with common methods for message stores
/// You should never directly use BaseMessageRepository
/// Use <see cref="CommandRepository"/>, <see cref="EventRepository{TEntityKey}"/> or <see cref="QueryRepository"/> instead
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public abstract class BaseMessageRepository<TMessage> :
    IMessageRepository<TMessage>,
    IMongoDbRepository
    where TMessage : class, IMessage
{
    protected readonly IMongoDatabase Database;
    protected readonly ILogger Logger;
    protected readonly InsertOneOptions InsertOneOptions;

    /// <summary>
    /// </summary>
    /// <param name="database"></param>
    /// <param name="logger"></param>
    /// <param name="insertOneOptions"></param>
    protected BaseMessageRepository(
        IMongoDatabase database,
        ILogger logger, InsertOneOptions? insertOneOptions = null
        )
    {
        Database = database;
        Logger = logger;
        InsertOneOptions = insertOneOptions ?? new InsertOneOptions();
    }

    public abstract string CollectionName { get; }

    protected virtual IMongoCollection<IMessage> BaseMessageCollection
        => Database.GetCollection<IMessage>(CollectionName);

    public abstract Task<TMessage?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default);

    public virtual async Task InsertAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        if (message.IsReplay) return;

        if (GenericUtils.IsNullOrDefault(message.Id)) message.Id = Guid.NewGuid();
        await BaseMessageCollection.InsertOneAsync(message, InsertOneOptions, cancellationToken);
    }

    public abstract Task UpsertHandlerAsync(Guid id, Type handlerType, Exception? exception = null, CancellationToken cancellationToken = default);

    public abstract Task EnsureCollectionIndexesAsync(CancellationToken cancellationToken = default);
}

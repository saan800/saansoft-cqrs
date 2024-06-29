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
    IMongoDbRepository
    where TMessageId : struct
    where TMessage : class, IMessage<TMessageId>
{
    protected readonly IMongoDatabase Database;
    protected readonly IIdGenerator<TMessageId> IdGenerator;
    protected readonly InsertManyOptions InsertManyOptions;

    /// <summary>
    /// </summary>
    /// <param name="database"></param>
    /// <param name="idGenerator"></param>
    /// <param name="insertManyOptions"></param>
    protected BaseMessageRepository(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, InsertManyOptions? insertManyOptions = null)
    {
        Database = database;
        IdGenerator = idGenerator;
        InsertManyOptions = insertManyOptions ?? new InsertManyOptions();
    }

    public abstract string CollectionName { get; }

    protected virtual IMongoCollection<IMessage<TMessageId>> BaseMessageCollection
        => Database.GetCollection<IMessage<TMessageId>>(CollectionName);

    public virtual async Task InsertAsync(TMessage message, CancellationToken cancellationToken = default)
        => await InsertManyAsync([message], cancellationToken);

    public virtual async Task InsertManyAsync(IEnumerable<TMessage> message, CancellationToken cancellationToken = default)
    {
        var localMessages = message.Where(x => !x.IsReplay).ToList();
        if (localMessages.Count == 0) return;

        localMessages = localMessages
            .Select(msg =>
            {
                if (GenericUtils.IsNullOrDefault(msg.Id)) msg.Id = IdGenerator.NewId();
                return msg;
            })
            .ToList();

        await BaseMessageCollection.InsertManyAsync(localMessages, InsertManyOptions, cancellationToken);
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the MessageCollection
    /// </summary>
    public virtual async Task EnsureCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<IMessage<TMessageId>>.IndexKeys
            .Ascending(x => x.TypeFullName)
            .Ascending(x => x.MessageOnUtc)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.TriggeredByUser);

        var indexModel =
            new CreateIndexModel<IMessage<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = false, Background = false });

        await BaseMessageCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }
}

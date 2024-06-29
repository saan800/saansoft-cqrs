// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

/// <summary>
/// Base class with common methods for message publisher stores
/// You should never directly use BasePublisherRepository
/// Use <see cref="CommandPublisherRepository{TMessageId}"/>, <see cref="EventPublisherRepository{TMessageId}"/> or <see cref="QueryPublisherRepository{TMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public abstract class BasePublisherRepository<TMessageId, TMessage> :
    IMessagePublisherRepository<TMessageId, TMessage>,
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
    /// <param name="replaceOptions">IsUpsert is forced to be true, but can specify other parameters as desired</param>
    protected BasePublisherRepository(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null)
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

    public virtual IMongoCollection<MessagePublisherRecord<TMessageId>> PublisherCollection
        => Database.GetCollection<MessagePublisherRecord<TMessageId>>(CollectionName);

    public virtual async Task UpsertPublisherAsync(IMessage<TMessageId> message, Type publisherType, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var messageTypeName = messageType.FullName;
        var messageAssembly = messageType.Assembly.GetName().Name ?? messageType.Assembly.GetName().FullName;
        var publisherTypeName = publisherType.FullName ?? publisherType.Name;
        var publisherAssembly = publisherType.Assembly.GetName().Name ?? publisherType.Assembly.GetName().FullName;
        if (string.IsNullOrWhiteSpace(messageTypeName) || string.IsNullOrWhiteSpace(publisherTypeName)) return;

        var record = await PublisherCollection.Find(x =>
                                x.MessageTypeName == messageTypeName &&
                                x.MessageAssembly == messageAssembly &&
                                x.PublisherTypeName == publisherTypeName &&
                                x.PublisherAssembly == publisherAssembly
                            )
                            .FirstOrDefaultAsync(cancellationToken);
        if (record == null)
        {
            record = new MessagePublisherRecord<TMessageId>
            {
                Id = IdGenerator.NewId(),
                MessageTypeName = messageTypeName,
                MessageAssembly = messageAssembly,
                PublisherTypeName = publisherTypeName,
                PublisherAssembly = publisherAssembly,
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

        await PublisherCollection.ReplaceOneAsync(
            x => x.MessageTypeName == messageTypeName &&
                 x.MessageAssembly == messageAssembly &&
                 x.PublisherTypeName == publisherTypeName &&
                 x.PublisherAssembly == publisherAssembly,
            record,
            ReplaceOptions,
            cancellationToken
        );
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the PublisherCollection
    /// </summary>
    public virtual async Task EnsureCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<MessagePublisherRecord<TMessageId>>.IndexKeys
            .Ascending(x => x.MessageTypeName)
            .Ascending(x => x.MessageAssembly)
            .Ascending(x => x.PublisherTypeName)
            .Ascending(x => x.PublisherAssembly);

        var indexModel =
            new CreateIndexModel<MessagePublisherRecord<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = true, Background = true });

        await PublisherCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }
}

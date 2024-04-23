using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Store.MongoDB;

/// <summary>
/// Base class with common methods for all message stores
/// You should never directly inherit from IMessageStore
/// Use <see cref="CommandStore{TMessageId}"/>, <see cref="EventStore{TMessageId, TEntityKey}"/> or <see cref="QueryStore{IMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public abstract class BaseMessageStore<TMessageId, TMessage>(IMongoDatabase database) :
    IMessageStore<TMessageId, TMessage>
    where TMessageId : struct
    where TMessage : class, IMessage<TMessageId>
{
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly IMongoDatabase Database = database;

    public abstract string MessageCollectionName { get; }

    public IMongoCollection<IMessage<TMessageId>> MessageCollection => Database.GetCollection<IMessage<TMessageId>>(MessageCollectionName);

    public async Task InsertAsync(TMessage message, CancellationToken cancellationToken = default)
        => await InsertManyAsync([message], cancellationToken);

    public async Task InsertManyAsync(IEnumerable<TMessage> message, CancellationToken cancellationToken = default)
    {
        var localMessages = message.Where(x => !x.IsReplay).ToList();
        if (localMessages.Count == 0) return;

        await MessageCollection.InsertManyAsync(localMessages, new InsertManyOptions(), cancellationToken);
    }

    /// <summary>
    /// Call this on your app startup to ensure that the necessary indexes are created
    /// </summary>
    public virtual async Task EnsureIndexes(CancellationToken cancellationToken = default)
    {
        var indexes = MessageCollection.Indexes;

        var keyIndex = Builders<IMessage<TMessageId>>.IndexKeys
            .Ascending(x => x.TypeFullName)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.AuthenticationId)
            .Ascending(x => x.MessageOnUtc);

        var indexModel =
            new CreateIndexModel<IMessage<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = false, Background = false });

        // await indexes.CreateOneAsync(
        //     Builders<IMessage<TMessageId>>.IndexKeys.Ascending(_ => _.TypeFullName).Descending(_ => _.TriggeredById),
        //     new CreateIndexOptions { Background = true },
        //     cancellationToken);

        await indexes.CreateOneAsync(
            indexModel,
                new CreateOneIndexOptions { },
                cancellationToken
            );
    }
}

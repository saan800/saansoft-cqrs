using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store.MongoDB.Models;
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
    IMessagePublisherStore,
    IMessageStore<TMessageId, TMessage>
    where TMessageId : struct
    where TMessage : class, IMessage<TMessageId>
{
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly IMongoDatabase Database = database;

    protected abstract TMessageId NewMessageId();

    public abstract string MessageCollectionName { get; }
    public abstract string PublisherCollectionName { get; }

    public IMongoCollection<IMessage<TMessageId>> MessageCollection => Database.GetCollection<IMessage<TMessageId>>(MessageCollectionName);
    public IMongoCollection<MessagePublisherRecord<TMessageId>> PublisherCollection => Database.GetCollection<MessagePublisherRecord<TMessageId>>(PublisherCollectionName);


    #region IMessageStore<TMessageId, TMessage>

    public async Task InsertAsync(TMessage message, CancellationToken cancellationToken = default)
        => await InsertManyAsync([message], cancellationToken);

    public async Task InsertManyAsync(IEnumerable<TMessage> message, CancellationToken cancellationToken = default)
    {
        var localMessages = message.Where(x => !x.IsReplay).ToList();
        if (localMessages.Count == 0) return;

        localMessages = localMessages
            .Select(msg =>
            {
                if (GenericUtils.IsNullOrDefault(msg.Id)) msg.Id = NewMessageId();
                return msg;
            })
            .ToList();

        await MessageCollection.InsertManyAsync(localMessages, new InsertManyOptions(), cancellationToken);
    }

    #endregion

    #region IMessagePublisherStore

    public async Task UpsertPublisherAsync(string messageTypeName, string publisherTypeName, CancellationToken cancellationToken = default)
    {
        var record = await PublisherCollection
                         .Find(x => x.MessageTypeName == messageTypeName && x.PublisherTypeName == publisherTypeName)
                         .FirstOrDefaultAsync(cancellationToken: cancellationToken)
                    ?? new MessagePublisherRecord<TMessageId>
                    {
                        Id = NewMessageId(),
                        MessageTypeName = messageTypeName,
                        PublisherTypeName = publisherTypeName,
                        CreatedOn = DateTime.Now
                    };
        record.LastMessageOn = DateTime.Now;

        await PublisherCollection.ReplaceOneAsync(
            x => x.MessageTypeName == messageTypeName && x.PublisherTypeName == publisherTypeName,
            record,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken
        );
    }

    #endregion

    #region Create indexes

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the MessageCollection
    /// </summary>
    public virtual async Task EnsureMessageCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<IMessage<TMessageId>>.IndexKeys
            .Ascending(x => x.TypeFullName)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.AuthenticationId)
            .Ascending(x => x.MessageOnUtc);

        var indexModel =
            new CreateIndexModel<IMessage<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = false, Background = false });

        await MessageCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the PublisherCollection
    /// </summary>
    public virtual async Task EnsurePublisherCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<MessagePublisherRecord<TMessageId>>.IndexKeys
            .Ascending(x => x.MessageTypeName)
            .Ascending(x => x.PublisherTypeName);

        var indexModel =
            new CreateIndexModel<MessagePublisherRecord<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = true, Background = true });

        await PublisherCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }

    #endregion
}

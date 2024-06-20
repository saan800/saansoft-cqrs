using MongoDB.Driver;
using SaanSoft.Cqrs.Decorator.Store.Models;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Utilities;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

/// <summary>
/// Base class with common methods for all message stores
/// You should never directly inherit from IMessageStore
/// Use <see cref="CommandStore{TMessageId}"/>, <see cref="EventStore{TMessageId, TEntityKey}"/> or <see cref="QueryStore{IMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public abstract class BaseMessageStore<TMessageId, TMessage>(IMongoDatabase database) :
    IMessagePublisherStore<TMessageId>,
    IMessageSubscriberStore,
    IMessageStore<TMessageId, TMessage>
    where TMessageId : struct
    where TMessage : class, IMessage<TMessageId>
{
    protected readonly IMongoDatabase Database = database;

    protected abstract TMessageId NewMessageId();

    #region Db and Collection config

    public abstract string MessageCollectionName { get; }
    public abstract string PublisherCollectionName { get; }
    public abstract string SubscriberCollectionName { get; }

    public virtual IMongoCollection<IMessage<TMessageId>> MessageCollection
        => Database.GetCollection<IMessage<TMessageId>>(MessageCollectionName);
    public virtual IMongoCollection<MessagePublisherRecord<TMessageId>> PublisherCollection
        => Database.GetCollection<MessagePublisherRecord<TMessageId>>(PublisherCollectionName);
    public virtual IMongoCollection<MessageSubscriberRecord<TMessageId>> SubscriberCollection
        => Database.GetCollection<MessageSubscriberRecord<TMessageId>>(SubscriberCollectionName);

    #endregion

    #region IMessageStore<TMessageId, TMessage>

    public virtual async Task InsertAsync(TMessage message, CancellationToken cancellationToken = default)
        => await InsertManyAsync([message], cancellationToken);

    public virtual async Task InsertManyAsync(IEnumerable<TMessage> message, CancellationToken cancellationToken = default)
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


    public virtual async Task UpsertPublisherAsync<TMessage>(TMessage message, Type publisherType, CancellationToken cancellationToken = default)
       where TMessage : IMessage<TMessageId>
    {
        var now = DateTime.UtcNow;
        var messageType = typeof(TMessage);
        var messageTypeName = messageType.FullName ?? messageType.Name;
        var messageAssembly = messageType.Assembly.FullName ?? messageType.Assembly.GetName().FullName;
        var publisherTypeName = publisherType.FullName ?? publisherType.Name;
        var publisherAssembly = publisherType.Assembly.FullName ?? publisherType.Assembly.GetName().FullName;
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
                Id = NewMessageId(),
                MessageTypeName = messageTypeName,
                MessageAssembly = messageAssembly,
                PublisherTypeName = publisherTypeName,
                PublisherAssembly = publisherAssembly,
                CreatedOnUtc = now,
                LastMessageOnUtc = now,
                LastProcessedMessageId = message.Id
            };
        }
        else
        {
            record.LastMessageOnUtc = now;
            record.LastProcessedMessageId = message.Id;
        }

        await PublisherCollection.ReplaceOneAsync(
            x => x.MessageTypeName == messageTypeName &&
                 x.MessageAssembly == messageAssembly &&
                 x.PublisherTypeName == publisherTypeName &&
                 x.PublisherAssembly == publisherAssembly,
            record,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken
        );
    }

    #endregion

    #region IMessageSubscriberStore

    public virtual async Task UpsertSubscriberAsync(string messageTypeName, IEnumerable<string> subscriberClassTypeNames, CancellationToken cancellationToken = default)
    {
        var typeNames = subscriberClassTypeNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToList();

        if (typeNames.Count == 0) return;
        if (typeNames.Count == 1)
        {
            // only one subscriber - do more efficient mongo upsert
            var typeName = typeNames.First();

            var record = await SubscriberCollection
                             .Find(x => x.MessageTypeName == messageTypeName && x.SubscriberTypeName == typeName)
                             .FirstOrDefaultAsync(cancellationToken)
                         ?? new MessageSubscriberRecord<TMessageId>
                         {
                             Id = NewMessageId(),
                             MessageTypeName = messageTypeName,
                             SubscriberTypeName = typeName,
                             CreatedOnUtc = DateTime.UtcNow
                         };
            record.LastMessageOnUtc = DateTime.UtcNow;

            await SubscriberCollection.ReplaceOneAsync(
                x => x.MessageTypeName == messageTypeName && x.SubscriberTypeName == typeName,
                record,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken
            );
            return;
        }

        // multiple subscribers - find which ones to update, which to insert
        var records = await SubscriberCollection
                         .Find(x => x.MessageTypeName.Equals(messageTypeName, StringComparison.OrdinalIgnoreCase))
                         .Project(x => new { x.Id, x.SubscriberTypeName })
                         .ToListAsync(cancellationToken);

        var newRecords = new List<MessageSubscriberRecord<TMessageId>>();
        var existingRecords = new List<TMessageId>();

        foreach (var subscriberClassTypeName in typeNames)
        {
            var record = records.FirstOrDefault(x => x.SubscriberTypeName.Equals(subscriberClassTypeName, StringComparison.OrdinalIgnoreCase));
            if (record == null)
            {
                newRecords.Add(new MessageSubscriberRecord<TMessageId>
                {
                    Id = NewMessageId(),
                    MessageTypeName = messageTypeName,
                    SubscriberTypeName = subscriberClassTypeName,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastMessageOnUtc = DateTime.UtcNow
                });
            }
            else
            {
                existingRecords.Add(record.Id);
            }
        }

        if (newRecords.Count > 0)
        {
            await SubscriberCollection.InsertManyAsync(newRecords, new InsertManyOptions(), cancellationToken);
        }

        if (existingRecords.Count > 0)
        {
            await SubscriberCollection.UpdateManyAsync(
                x => existingRecords.Contains(x.Id),
                Builders<MessageSubscriberRecord<TMessageId>>.Update.Set(x => x.LastMessageOnUtc, DateTime.UtcNow),
                new UpdateOptions { IsUpsert = false },
                cancellationToken
                );
        }
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
            .Ascending(x => x.MessageOnUtc)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.AuthenticationId);

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
            .Ascending(x => x.MessageAssembly)
            .Ascending(x => x.PublisherTypeName)
            .Ascending(x => x.PublisherAssembly);

        var indexModel =
            new CreateIndexModel<MessagePublisherRecord<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = true, Background = true });

        await PublisherCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }

    /// <summary>
    /// Call in the app startup to ensure that the necessary indexes are created for the SubscriberCollection
    /// </summary>
    public virtual async Task EnsureSubscriberCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<MessageSubscriberRecord<TMessageId>>.IndexKeys
            .Ascending(x => x.MessageTypeName)
            .Ascending(x => x.SubscriberTypeName);

        var indexModel =
            new CreateIndexModel<MessageSubscriberRecord<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = true, Background = true });

        await SubscriberCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }

    #endregion
}

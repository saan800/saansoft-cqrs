using MongoDB.Driver;
using SaanSoft.Cqrs.Decorator.Store.Models;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Utilities;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

/// <summary>
/// Base class with common methods for all message stores
/// You should never directly inherit from IMessageRepository
/// Use <see cref="CommandRepository{TMessageId}"/>, <see cref="EventRepository{TMessageId,TEntityKey}"/> or <see cref="QueryRepository{TMessageId}"/> instead
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public abstract class BaseMessageRepository<TMessageId, TMessage>(IMongoDatabase database) :
    IMessagePublisherRepository<TMessageId, TMessage>,
    IMessageHandlerRepository<TMessageId, TMessage>,
    IMessageRepository<TMessageId, TMessage>
    where TMessageId : struct
    where TMessage : class, IMessage<TMessageId>
{
    protected readonly IMongoDatabase Database = database;

    protected abstract TMessageId NewMessageId();

    #region Db and Collection config

    public abstract string MessageCollectionName { get; }
    public abstract string PublisherCollectionName { get; }
    public abstract string HandlerCollectionName { get; }

    public virtual IMongoCollection<IMessage<TMessageId>> MessageCollection
        => Database.GetCollection<IMessage<TMessageId>>(MessageCollectionName);
    public virtual IMongoCollection<MessagePublisherRecord<TMessageId>> PublisherCollection
        => Database.GetCollection<MessagePublisherRecord<TMessageId>>(PublisherCollectionName);
    public virtual IMongoCollection<MessageHandlerRecord<TMessageId>> HandlerCollection
        => Database.GetCollection<MessageHandlerRecord<TMessageId>>(HandlerCollectionName);

    #endregion

    #region IMessageRepository<TMessageId, TMessage>

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
                Id = NewMessageId(),
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
            new ReplaceOptions { IsUpsert = true },
            cancellationToken
        );
    }

    #endregion

    #region IMessageHandlerStore

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
                Id = NewMessageId(),
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
            .Ascending(x => x.MessageOnUtc)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.TriggeredByUser);

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
    /// Call in the app startup to ensure that the necessary indexes are created for the HandlerCollection
    /// </summary>
    public virtual async Task EnsureHandlerCollectionIndexes(CancellationToken cancellationToken = default)
    {
        var keyIndex = Builders<MessageHandlerRecord<TMessageId>>.IndexKeys
            .Ascending(x => x.MessageTypeName)
            .Ascending(x => x.HandlerTypeName);

        var indexModel =
            new CreateIndexModel<MessageHandlerRecord<TMessageId>>(keyIndex, new CreateIndexOptions { Unique = true, Background = true });

        await HandlerCollection.Indexes.CreateOneAsync(indexModel, new CreateOneIndexOptions(), cancellationToken);
    }

    #endregion
}

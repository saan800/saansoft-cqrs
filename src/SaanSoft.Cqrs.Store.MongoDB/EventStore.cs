using MongoDB.Driver;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store.MongoDB;

public class EventStore(IMongoDatabase database)
    : EventStore<Guid>(database)
{
}

public class EventStore<TEntityKey>(IMongoDatabase database)
    : EventStore<Guid, TEntityKey>(database)
    where TEntityKey : struct
{
}

/// <summary>
///
/// </summary>
/// <remarks>
/// Ensure you add an index on the mongo collection's Key property
/// </remarks>
/// <param name="database"></param>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TEntityKey"></typeparam>
public abstract class EventStore<TMessageId, TEntityKey>(IMongoDatabase database) :
    BaseMessageStore<TMessageId, IEvent<TMessageId>>(database),
    IEventStore<TMessageId, TEntityKey>
    where TMessageId : struct
    where TEntityKey : struct
{
    public override string MessageCollectionName => "Events";

    protected IMongoCollection<IEvent<TMessageId, TEntityKey>> EventCollection =>
        Database.GetCollection<IEvent<TMessageId, TEntityKey>>(MessageCollectionName);

    public async Task<List<IEvent<TMessageId, TEntityKey>>> GetAsync(TEntityKey key,
        CancellationToken cancellationToken = default)
        => (await EventCollection
            .Find(x => x.Key.Equals(key))
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.MessageOnUtc).ToList();

    /// <summary>
    /// Call this on your app startup to ensure that the necessary indexes are created
    /// </summary>
    public override async Task EnsureIndexes(CancellationToken cancellationToken = default)
    {
        var indexes = EventCollection.Indexes;

        var keyIndex = Builders<IEvent<TMessageId, TEntityKey>>.IndexKeys
            .Ascending(x => x.Key)
            .Ascending(x => x.TypeFullName)
            .Ascending(x => x.TriggeredById)
            .Ascending(x => x.AuthenticationId)
            .Ascending(x => x.MessageOnUtc);

        await indexes.CreateOneAsync(
            new CreateIndexModel<IEvent<TMessageId, TEntityKey>>(keyIndex, new CreateIndexOptions { Unique = false }),
            null,
            cancellationToken
        );
    }
}

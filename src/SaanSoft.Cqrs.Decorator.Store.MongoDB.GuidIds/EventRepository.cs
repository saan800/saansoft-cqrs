namespace SaanSoft.Cqrs.Decorator.Store.MongoDB.GuidIds;

public interface IEventMongoDbRepository :
    IEventMongoDbRepository<Guid>,
    IEventRepository
{
}

public interface IEventMongoDbRepository<TEntityKey> :
    IEventMongoDbRepository<Guid, TEntityKey>,
    IEventRepository<TEntityKey>,
    IEventHandlerRepository
    where TEntityKey : struct
{
}

public class EventRepository(IMongoDatabase database, IIdGenerator idGenerator, ILogger logger, InsertOneOptions? insertOneOptions = null)
    : EventRepository<Guid>(database, idGenerator, logger, insertOneOptions),
      IEventMongoDbRepository
{
}

public class EventRepository<TEntityKey>(IMongoDatabase database, IIdGenerator idGenerator, ILogger logger, InsertOneOptions? insertOneOptions = null)
    : EventRepository<Guid, TEntityKey>(database, idGenerator, logger, insertOneOptions),
      IEventMongoDbRepository<TEntityKey>
    where TEntityKey : struct
{
}

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IEventHandlerMongoDbRepository<TMessageId> :
    IEventHandlerRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
}

public class EventHandlerRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ReplaceOptions? replaceOptions = null) :
    EventHandlerRepository<Guid>(database, idGenerator, replaceOptions)
{
}

public class EventHandlerRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null) :
    BaseHandlerRepository<TMessageId, IEvent<TMessageId>>(database, idGenerator, replaceOptions),
    IEventHandlerMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "EventHandlers";
}


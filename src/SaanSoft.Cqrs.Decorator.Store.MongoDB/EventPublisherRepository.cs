namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public interface IEventPublisherMongoDbRepository<TMessageId> :
    IEventPublisherRepository<TMessageId>,
    IMongoDbRepository
    where TMessageId : struct
{
}

public class EventPublisherRepository(IMongoDatabase database, IIdGenerator<Guid> idGenerator, ReplaceOptions? replaceOptions = null) :
    EventPublisherRepository<Guid>(database, idGenerator, replaceOptions)
{
}

public class EventPublisherRepository<TMessageId>(IMongoDatabase database, IIdGenerator<TMessageId> idGenerator, ReplaceOptions? replaceOptions = null) :
    BasePublisherRepository<TMessageId, IEvent<TMessageId>>(database, idGenerator, replaceOptions),
    IEventPublisherMongoDbRepository<TMessageId>
    where TMessageId : struct
{
    public override string CollectionName => "EventPublishers";
}

namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.GuidIds.Repositories;

public class EventHandlerRepositoryTests : BaseHandlerRepositoryTests<MyEvent, EventRepository, BaseEvent<Guid, Guid>>
{
    public EventHandlerRepositoryTests()
    {
        SutRepository = new EventRepository(Database, IdGenerator, Logger);
        MessageCollection = SutRepository.MessageCollection;
    }

    protected override MyEvent CreateNewMessage() => new MyEvent(Guid.NewGuid());
}

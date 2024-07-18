namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class EventHandlerRepositoryTests : BaseHandlerRepositoryTests<MyEvent, EventRepository, Event<Guid>>
{
    public EventHandlerRepositoryTests()
    {
        SutRepository = new EventRepository(Database, Logger);
        MessageCollection = SutRepository.MessageCollection;
    }

    protected override MyEvent CreateNewMessage() => new MyEvent(Guid.NewGuid());
}

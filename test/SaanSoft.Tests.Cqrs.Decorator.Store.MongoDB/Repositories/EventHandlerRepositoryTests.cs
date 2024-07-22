namespace SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB.Repositories;

public class EventHandlerRepositoryTests : BaseHandlerRepositoryTests<IEvent, EventRepository>
{
    public EventHandlerRepositoryTests()
    {
        SutRepository = new EventRepository(Database, Logger);
    }

    protected override MyEvent CreateNewMessage() => new MyEvent(Guid.NewGuid());
}

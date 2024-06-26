namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class EventsHandler :
    IEventHandler<MyEvent>,
    IEventHandler<AnotherEvent>
{
    public Task HandleAsync(MyEvent evt, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task HandleAsync(AnotherEvent evt, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

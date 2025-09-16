using SaanSoft.Cqrs.Handlers;

namespace SaanSoft.Tests.Cqrs.Shared.TestHandlers;

public class EventsHandler :
    IEventHandler<MyEvent>,
    IEventHandler<AnotherEvent>
{
    public Task HandleAsync(MyEvent evt, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task HandleAsync(AnotherEvent evt, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class EventsHandler :
    IBaseEventHandler<MyEvent>,
    IBaseEventHandler<AnotherEvent>
{
    public Task HandleAsync(MyEvent evt, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task HandleAsync(AnotherEvent evt, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

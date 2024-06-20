using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Common.TestSubscribers;

/// <summary>
/// Test event subscriber bus -  useful when testing decorators
/// </summary>
/// <param name="serviceProvider"></param>
public class TestEventSubscriber(IServiceProvider serviceProvider) : IEventSubscriber<Guid>
{
    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<Guid>
    {
        // run each group of handlers in the given priority order
        foreach (var tasks in GetHandlers<TEvent>()
                     .Select(group => group.Select(handler => RunAsync(evt, handler, cancellationToken)))
                )
        {
            await Task.WhenAll(tasks);
        }
    }

    public Task RunAsync<TEvent>(TEvent evt, IEventHandler<TEvent> handler,
        CancellationToken cancellationToken = default) where TEvent : IEvent<Guid>
        => handler.HandleAsync(evt, cancellationToken);


    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>() where TEvent : IEvent<Guid>
        => serviceProvider.GetPrioritisedEventHandlers<TEvent, Guid>();
}

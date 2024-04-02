using Microsoft.Extensions.DependencyInjection;

using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class LocalEventBus(IServiceProvider serviceProvider)
    : LocalEventBus<Guid>(serviceProvider);

public abstract class LocalEventBus<TMessageId>(IServiceProvider serviceProvider)
    : IEventBus<TMessageId>
    where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
        => await QueueManyAsync([evt], cancellationToken);

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        var services = serviceProvider.GetServices<IEventHandler<TEvent>>()?.ToList() ?? [];
        foreach (var evt in events)
        {
            foreach (var service in services)
            {
                await service.HandleAsync(evt, cancellationToken);
            }
        }
    }
}

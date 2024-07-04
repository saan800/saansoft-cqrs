using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Decorator.EnsureMessageId;

/// <summary>
/// Ensure that the Event has the Id field populated with a non-null and non-default value
/// </summary>
/// <param name="idGenerator"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
/// <typeparam name="TEntityKey"></typeparam>
public abstract class EnsureEventHasIdDecorator<TMessageId, TEntityKey>(IIdGenerator<TMessageId> idGenerator, IEventBus<TMessageId> next)
    : IEventBusDecorator<TMessageId>
    where TMessageId : struct
    where TEntityKey : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(evt.Id)) evt.Id = idGenerator.NewId();
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TMessageId>
    {
        var eventList = events.ToList();
        foreach (var evt in eventList.Where(evt => GenericUtils.IsNullOrDefault(evt.Id)).ToList())
        {
            evt.Id = idGenerator.NewId();
        }
        await next.QueueManyAsync(eventList, cancellationToken);
    }
}

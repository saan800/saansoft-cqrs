namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

/// <summary>
/// Ensure that the Event has the CorrelationId field populated with a non-null and non-default value
/// </summary>
/// <param name="providers"></param>
/// <param name="next"></param>
public class EnsureEventHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, IEventBus next)
    : IEventBus
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : class, IEvent
    {
        evt.CorrelationId = providers.EnsureCorrelationId(evt.CorrelationId);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : class, IEvent
    {
        // only do this if any events don't have correlationIds
        var eventList = events.ToList();
        if (eventList.Any(evt => string.IsNullOrWhiteSpace(evt.CorrelationId)))
        {
            // check if there are any correlationIds already populated
            var existingCorrelationIds = eventList
                .Where(evt => !string.IsNullOrWhiteSpace(evt.CorrelationId))
                .Select(evt => evt.CorrelationId ?? string.Empty)
                .Distinct()
                .ToList();

            // if there is exactly one correlationIds - then use that for all events
            var correlationId = existingCorrelationIds.Count == 1
                ? existingCorrelationIds.First()
                // otherwise generate a new correlationId for the events that don't have one
                : providers.EnsureCorrelationId(null);

            foreach (var evt in eventList.Where(evt => string.IsNullOrWhiteSpace(evt.CorrelationId)).ToList())
            {
                evt.CorrelationId = correlationId;
            }
        }

        await next.QueueManyAsync(eventList, cancellationToken);
    }
}

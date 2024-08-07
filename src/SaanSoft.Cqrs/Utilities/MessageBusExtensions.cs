using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Utilities;

public static class MessageBusExtensions
{
    /// <summary>
    /// Get handlers for an event, grouped and ordered by handler priority
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static List<IGrouping<int, IEventHandler<TEvent>>> GetPrioritisedEventHandlers<TEvent>(this IServiceProvider serviceProvider)
        where TEvent : IEvent
    {
        return serviceProvider
            .GetServices<IEventHandler<TEvent>>()
            // TODO: create optional attribute on event handlers to indicate running priority,
            // default priority = 0
            .GroupBy(_ => 0)
            .OrderBy(x => x.Key) // return handler groups in priority order
            .ToList();
    }
}

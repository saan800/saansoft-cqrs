using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Utilities;

public static class MessageBusExtensions
{
    /// <summary>
    /// Get handlers for an event, grouped and ordered by handler priority
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TMessageId"></typeparam>
    /// <returns></returns>
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static List<IGrouping<int, IEventHandler<TEvent>>> GetPrioritisedEventHandlers<TEvent, TMessageId>(this IServiceProvider serviceProvider)
        where TEvent : IEvent<TMessageId>
        where TMessageId : struct
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

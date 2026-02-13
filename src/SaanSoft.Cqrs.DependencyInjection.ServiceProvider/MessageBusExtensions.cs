using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.DependencyInjection.ServiceProvider;

public static class MessageBusExtensions
{
    // TODO: remove this??

    /// <summary>
    /// Get handlers for an event, grouped and ordered by handler priority
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public static List<IGrouping<int, IHandleMessage<TEvent>>> GetPrioritisedEventHandlers<TEvent>(
        this IServiceProvider serviceProvider)
        where TEvent : IEvent
    {
        return serviceProvider
            .GetServices<IHandleMessage<TEvent>>()
            // TODO: create optional attribute on event handlers to indicate running priority,
            // default priority = 0
            .GroupBy(_ => 0)
            .OrderBy(x => x.Key) // return handler groups in priority order
            .ToList();
    }
}

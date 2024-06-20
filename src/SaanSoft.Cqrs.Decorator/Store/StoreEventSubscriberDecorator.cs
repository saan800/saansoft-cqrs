using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreEventSubscriberDecorator(IServiceProvider serviceProvider, IEventSubscriberStore store, IEventSubscriber<Guid> next)
    : StoreEventSubscriberDecorator<Guid>(serviceProvider, store, next);

public abstract class StoreEventSubscriberDecorator<TMessageId>(IServiceProvider serviceProvider, IEventSubscriberStore store, IEventSubscriber<TMessageId> next) :
    BaseStoreMessageSubscriberDecorator(serviceProvider, store),
    IEventSubscriber<TMessageId> where TMessageId : struct
{
    protected override bool AllowMultipleSubscribers => true;

    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        await StoreSubscriber<TEvent, IEventHandler<TEvent>>(cancellationToken);
        await next.RunAsync(evt, cancellationToken);
    }
}

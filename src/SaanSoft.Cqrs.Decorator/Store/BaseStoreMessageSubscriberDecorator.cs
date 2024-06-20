using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageSubscriberDecorator(IServiceProvider serviceProvider, IMessageSubscriberStore store) : ISubscriberDecorator
{
    protected abstract bool AllowMultipleSubscribers { get; }

    protected async Task StoreSubscriber<TMessage, TSubscriber>(CancellationToken cancellationToken) where TSubscriber : notnull
    {
        var messageType = typeof(TMessage);
        var handlers = serviceProvider.GetServices<TSubscriber>().ToList();
        if (handlers.Count == 0 || (handlers.Count > 1 && !AllowMultipleSubscribers)) return;

        var messageTypeName = messageType.FullName ?? messageType.Name;
        var subscriberNames = handlers.Select(x =>
        {
            var type = x.GetType();
            return type.FullName ?? type.Name;
        })
        .ToList();

        await store.UpsertSubscriberAsync(messageTypeName, subscriberNames, cancellationToken);
    }
}


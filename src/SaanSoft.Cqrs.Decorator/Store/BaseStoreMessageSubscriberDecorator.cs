using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageSubscriberDecorator<TMessageId, TMessage>(IMessageSubscriberStore<TMessageId, TMessage> store)
    : ISubscriberDecorator
    where TMessage : IMessage<TMessageId>
    where TMessageId : struct
{
    protected readonly IMessageSubscriberStore<TMessageId, TMessage> Store = store;
}


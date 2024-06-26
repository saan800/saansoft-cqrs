namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageHandlerDecorator<TMessageId, TMessage>(IMessageHandlerRepository<TMessageId, TMessage> repository)
    : IMessageSubscriptionBus
    where TMessage : IMessage<TMessageId>
    where TMessageId : struct
{
    protected readonly IMessageHandlerRepository<TMessageId, TMessage> Repository = repository;
}


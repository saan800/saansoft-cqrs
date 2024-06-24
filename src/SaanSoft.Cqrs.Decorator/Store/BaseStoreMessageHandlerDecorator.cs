using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageHandlerDecorator<TMessageId, TMessage>(IMessageHandlerRepository<TMessageId, TMessage> repository)
    : ISubscriptionBusDecorator
    where TMessage : IMessage<TMessageId>
    where TMessageId : struct
{
    protected readonly IMessageHandlerRepository<TMessageId, TMessage> Repository = repository;
}


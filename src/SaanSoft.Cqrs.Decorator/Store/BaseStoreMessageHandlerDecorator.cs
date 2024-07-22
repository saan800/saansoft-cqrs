namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageHandlerDecorator<TMessage>(
    IMessageRepository<TMessage> repository) :
    IMessageSubscriptionBus
    where TMessage : IMessage
{
    protected readonly IMessageRepository<TMessage> Repository = repository;
}


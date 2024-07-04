namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageHandlerDecorator<TMessageId>(IMessageHandlerRepository<TMessageId> repository)
    : IMessageSubscriptionBus
    where TMessageId : struct
{
    protected readonly IMessageHandlerRepository<TMessageId> Repository = repository;
}


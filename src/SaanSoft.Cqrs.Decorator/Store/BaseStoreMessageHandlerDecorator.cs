namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageHandlerDecorator(IMessageHandlerRepository repository)
    : IMessageSubscriptionBus
{
    protected readonly IMessageHandlerRepository Repository = repository;
}


namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class BaseStoreMessageHandlerDecorator<TMessageId>(IMessageHandlerRepository<TMessageId> repository) :
    IBaseBus
    where TMessageId : struct
{
    protected readonly IMessageHandlerRepository<TMessageId> Repository = repository;
}


namespace SaanSoft.Cqrs.Decorator.Store;

public interface IEventPublisherStore<TMessageId> : IMessagePublisherStore<TMessageId>
    where TMessageId : struct
{
}

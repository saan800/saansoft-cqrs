namespace SaanSoft.Cqrs.Decorator.Store;

public interface IQueryPublisherStore<TMessageId> : IMessagePublisherStore<TMessageId>
    where TMessageId : struct
{
}

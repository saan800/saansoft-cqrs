namespace SaanSoft.Cqrs.Decorator.Store;

public interface IQueryPublisherRepository<TMessageId> : IMessagePublisherRepository<TMessageId, IQuery<TMessageId>>
    where TMessageId : struct
{
}

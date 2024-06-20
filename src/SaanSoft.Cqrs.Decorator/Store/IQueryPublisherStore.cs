using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface IQueryPublisherStore<TMessageId> : IMessagePublisherStore<TMessageId, IQuery<TMessageId>>
    where TMessageId : struct
{
}

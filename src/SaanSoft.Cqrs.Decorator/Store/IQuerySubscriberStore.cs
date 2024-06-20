using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface IQuerySubscriberStore<TMessageId> : IMessageSubscriberStore<TMessageId, IQuery<TMessageId>>
    where TMessageId : struct
{
}

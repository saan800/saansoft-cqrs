using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface IEventSubscriberStore<TMessageId> : IMessageSubscriberStore<TMessageId, IEvent<TMessageId>>
    where TMessageId : struct
{
}

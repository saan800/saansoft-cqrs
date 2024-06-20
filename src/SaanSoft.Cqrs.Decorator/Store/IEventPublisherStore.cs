using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface IEventPublisherStore<TMessageId> : IMessagePublisherStore<TMessageId, IEvent<TMessageId>>
    where TMessageId : struct
{
}

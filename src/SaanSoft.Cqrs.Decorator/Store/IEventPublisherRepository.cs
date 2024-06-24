using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface IEventPublisherRepository<TMessageId> : IMessagePublisherRepository<TMessageId, IEvent<TMessageId>>
    where TMessageId : struct
{
}

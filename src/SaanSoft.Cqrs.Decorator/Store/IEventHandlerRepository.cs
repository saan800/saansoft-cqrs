namespace SaanSoft.Cqrs.Decorator.Store;

public interface IEventHandlerRepository<TMessageId> : IMessageHandlerRepository<TMessageId, IEvent<TMessageId>>
    where TMessageId : struct
{
}

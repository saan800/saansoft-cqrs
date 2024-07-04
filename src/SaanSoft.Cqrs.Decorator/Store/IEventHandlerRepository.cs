namespace SaanSoft.Cqrs.Decorator.Store;

public interface IEventHandlerRepository<in TMessageId> : IMessageHandlerRepository<TMessageId>
    where TMessageId : struct
{
}

namespace SaanSoft.Cqrs.Decorator.Store;

public interface IQueryHandlerRepository<in TMessageId> : IMessageHandlerRepository<TMessageId>
    where TMessageId : struct
{
}

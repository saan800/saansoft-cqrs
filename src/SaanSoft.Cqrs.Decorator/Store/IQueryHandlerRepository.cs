namespace SaanSoft.Cqrs.Decorator.Store;

public interface IQueryHandlerRepository<TMessageId> : IMessageHandlerRepository<TMessageId, IQuery<TMessageId>>
    where TMessageId : struct
{
}

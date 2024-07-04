namespace SaanSoft.Cqrs.Decorator.Store;

public interface ICommandHandlerRepository<in TMessageId> : IMessageHandlerRepository<TMessageId>
    where TMessageId : struct
{
}

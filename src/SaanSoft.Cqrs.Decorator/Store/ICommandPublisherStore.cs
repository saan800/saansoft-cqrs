namespace SaanSoft.Cqrs.Decorator.Store;

public interface ICommandPublisherStore<TMessageId> : IMessagePublisherStore<TMessageId>
    where TMessageId : struct
{
}

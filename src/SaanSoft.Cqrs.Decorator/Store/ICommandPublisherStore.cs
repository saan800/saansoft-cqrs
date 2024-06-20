using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface ICommandPublisherStore<TMessageId> : IMessagePublisherStore<TMessageId, ICommand<TMessageId>>
    where TMessageId : struct
{
}

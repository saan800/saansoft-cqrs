using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface ICommandSubscriberStore<TMessageId> : IMessageSubscriberStore<TMessageId, ICommand<TMessageId>>
    where TMessageId : struct
{
}

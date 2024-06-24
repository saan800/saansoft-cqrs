using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface ICommandPublisherRepository<TMessageId> : IMessagePublisherRepository<TMessageId, ICommand<TMessageId>>
    where TMessageId : struct
{
}
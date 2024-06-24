using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public interface ICommandHandlerRepository<TMessageId> : IMessageHandlerRepository<TMessageId, ICommand<TMessageId>>
    where TMessageId : struct
{
}

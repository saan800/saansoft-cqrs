using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store;

/// <summary>
/// ICommandStore is primarily useful for building an audit log and/or debugging
/// Its not actually used anywhere in SaanSoft.Cqrs
/// </summary>
public interface ICommandStore<TMessageId> : IMessageStore<TMessageId, ICommand<TMessageId>>
    where TMessageId : struct
{
}

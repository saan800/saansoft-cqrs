using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store;

/// <summary>
/// IQueryStore is primarily useful for building an audit log and/or debugging
/// Its not actually used anywhere in SaanSoft.Cqrs
/// </summary>
public interface IQueryStore<TMessageId> : IMessageStore<TMessageId, IQuery<TMessageId>>
    where TMessageId : struct
{
}

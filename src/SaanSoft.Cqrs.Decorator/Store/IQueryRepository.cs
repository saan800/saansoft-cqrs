namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// IQueryRepository is primarily useful for building an audit log and/or debugging
/// Its not actually used anywhere in SaanSoft.Cqrs
/// </summary>
public interface IQueryRepository<TMessageId> : IMessageRepository<TMessageId, IBaseQuery<TMessageId>>
    where TMessageId : struct
{
}

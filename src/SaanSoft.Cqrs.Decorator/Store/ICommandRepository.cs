namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// ICommandRepository is primarily useful for building an audit log and/or debugging
/// Its not actually used anywhere in SaanSoft.Cqrs
/// </summary>
public interface ICommandRepository : IMessageRepository<IBaseCommand>
{
}

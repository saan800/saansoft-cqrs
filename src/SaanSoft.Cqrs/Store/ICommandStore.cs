using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store;

/// <summary>
/// ICommandStore is primarily useful for building an audit log and/or debugging
/// Its not actually used anywhere in SaanSoft.Cqrs
/// </summary>
public interface ICommandStore<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Save a new command
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task InsertAsync<TCommand>(TCommand command) where TCommand : ICommand<TMessageId>;
}

using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store;

/// <summary>
/// ICommandStore is primarily useful for building an audit log and/or debugging
/// Its not actually used anywhere in SaanSoft.Cqrs
/// </summary>
public interface ICommandStore<TMessageId>
{
    /// <summary>
    /// Save a new command
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task InsertAsync<TCommand>(TCommand command) where TCommand : ICommand<TMessageId>;

    /// <summary>
    /// Save new commands
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    Task InsertManyAsync<TCommand>(IEnumerable<TCommand> commands) where TCommand : ICommand<TMessageId>;
}

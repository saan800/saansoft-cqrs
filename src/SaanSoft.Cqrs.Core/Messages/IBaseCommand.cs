namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseCommand{TMessageId}"/> and <see cref="IBaseCommand{TMessageId,TCommand,TResponse}"/> instead.
/// </summary>
public interface IBaseCommand : IBaseMessage
{
}

/// <summary>
/// Because we have both ICommand{TMessageId} and ICommand{TMessageId, TCommand, TResponse} that
/// sometimes can be handled the same way, but other times need to differentiate them.
///
/// You should only use this interface where you explicitly need to handle commands that do, and do not return results
/// Normally should use <see cref="IBaseCommand{TMessageId}"/> and <see cref="IBaseCommand{TMessageId,TCommand,TResponse}"/> instead.
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
public interface IRootCommand<TMessageId> : IBaseCommand, IBaseMessage<TMessageId>
    where TMessageId : struct
{
}

public interface IBaseCommand<TMessageId> : IRootCommand<TMessageId>
    where TMessageId : struct
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseCommand{TMessageId,TCommand,TResponse}"/> instead
/// </summary>
public interface IBaseCommand<TCommand, TResponse> : IBaseCommand
    where TCommand : IBaseCommand<TCommand, TResponse>
{
}

public interface IBaseCommand<TMessageId, TCommand, TResponse> :
    IBaseCommand<TCommand, TResponse>,
    IRootCommand<TMessageId>
    where TMessageId : struct
    where TCommand : IBaseCommand<TMessageId, TCommand, TResponse>
{
}

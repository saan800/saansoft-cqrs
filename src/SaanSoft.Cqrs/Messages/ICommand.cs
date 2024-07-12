namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="ICommand{TMessageId}"/> and <see cref="ICommand{TMessageId, TCommand, TResponse}"/> instead.
/// </summary>
public interface ICommand : IMessage
{
}

/// <summary>
/// Because we have both ICommand{TMessageId} and ICommand{TMessageId, TCommand, TResponse} that
/// sometimes can be handled the same way, but other times need to differentiate them.
///
/// You should never directly inherit from this interface
/// use <see cref="ICommand{TMessageId}"/> and <see cref="ICommand{TMessageId, TCommand, TResponse}"/> instead.
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
public interface IBaseCommand<TMessageId> : ICommand, IMessage<TMessageId>
    where TMessageId : struct
{
}

public interface ICommand<TMessageId> : IBaseCommand<TMessageId>
    where TMessageId : struct
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="ICommand{TMessageId, TCommand, TResponse}"/> instead
/// </summary>
public interface ICommand<TCommand, TResponse> : ICommand
    where TCommand : ICommand<TCommand, TResponse>
{
}

public interface ICommand<TMessageId, TCommand, TResponse> :
    ICommand<TCommand, TResponse>,
    IBaseCommand<TMessageId>
    where TMessageId : struct
    where TCommand : ICommand<TMessageId, TCommand, TResponse>
{
}

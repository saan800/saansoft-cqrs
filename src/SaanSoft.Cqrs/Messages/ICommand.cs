namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="ICommand{TMessageId}"/> instead
/// </summary>
public interface ICommand : IMessage
{
}

public interface ICommand<TMessageId> : ICommand, IMessage<TMessageId>
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

public interface ICommand<TMessageId, TCommand, TResponse> : ICommand<TCommand, TResponse>, ICommand<TMessageId>
    where TMessageId : struct
    where TCommand : ICommand<TMessageId, TCommand, TResponse>
{
}

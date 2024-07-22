namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Because we have both ICommand and ICommand{TCommand, TResponse} that
/// sometimes can be handled the same way, but other times need to differentiate them.
///
/// You should never directly inherit from this interface
/// use <see cref="ICommand"/> and <see cref="ICommand{TCommand, TResponse}"/> instead.
/// </summary>
public interface IBaseCommand : IMessage
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="ICommand"/> and <see cref="ICommand{TCommand, TResponse}"/> instead.
/// </summary>
public interface ICommand : IBaseCommand
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="ICommand{TCommand, TResponse}"/> instead
/// </summary>
public interface ICommand<TCommand, TResponse> : IBaseCommand
    where TCommand : ICommand<TCommand, TResponse>
{
}


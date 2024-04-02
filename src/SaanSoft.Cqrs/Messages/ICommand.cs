namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="ICommand{TMessageKey}"/> instead
/// </summary>
public interface ICommand : IMessage
{
}

public interface ICommand<TMessageId> : ICommand, IMessage<TMessageId>
    where TMessageId : struct
{
}

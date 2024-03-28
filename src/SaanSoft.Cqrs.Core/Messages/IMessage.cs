namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// Base class with common properties for all messages
/// You should never directly inherit from IMessage
/// Use <see cref="ICommand"/>, <see cref="IEvent{TId, TEntityKey}"/> or <see cref="IQuery{TQuery, TResult}"/> insteead
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Used to track related commands/events/queries
    /// Should be propagated between related messages
    /// </summary>
    string CorrelationId { get; set; }

    /// <summary>
    /// Who triggered the command/event/query (eg UserId, AuthToken)
    /// Should be propagated between related messages
    /// </summary>
    string? AuthenticatedId { get; set; }
}

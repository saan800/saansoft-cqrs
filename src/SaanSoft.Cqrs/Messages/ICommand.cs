namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Command without a response.
///
/// Command handling should not alter any state in the system.
/// The handler will often raise one or more associated events.
/// </summary>
/// <remarks>
/// Names should be in the form of a verb, eg CreateOrder, UpdateUserDetails, DeleteBlogPost
/// </remarks>
public interface ICommand : IMessage;

/// <summary>
/// Command with a response
///
/// Command handling should not alter any state in the system.
/// The handler will often raise one or more associated events.
///
/// Returns the result of the command
/// </summary>
/// <remarks>
/// Names should be in the form of a verb, eg CreateOrder, UpdateUserDetails, DeleteBlogPost
/// </remarks>
public interface ICommand<TResult> : IMessage<TResult>;

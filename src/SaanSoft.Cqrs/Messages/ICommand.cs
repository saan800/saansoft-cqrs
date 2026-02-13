namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Command without a response.
///
/// Handling the command should including validation and other business logic to ensure its valid to continue.
/// Command handling should not alter any state in the system.
/// The handler will often raise one or more associated events.
/// </summary>
/// <remarks>
/// Names should be in the form of a verb, eg CreateOrder, UpdateUserDetails, DeleteBlogPost
/// </remarks>
public interface ICommand : IMessage;

/// <summary>
/// Command with a response.
///
/// Normally commands shouldn't have responses but sometimes when integrating with
/// third party applications (eg payment provider) it's useful to return response data to the caller.
///
/// Handling the command should including validation and other business logic to ensure its valid to continue.
/// Command handling should not alter any state in the system.
/// The handler will often raise one or more associated events.
///
/// Returns the response of the command
/// </summary>
/// <remarks>
/// Names should be in the form of a verb, eg CreateOrder, UpdateUserDetails, DeleteBlogPost
/// </remarks>
public interface ICommand<TResponse> : ICommand, IMessage<TResponse>;

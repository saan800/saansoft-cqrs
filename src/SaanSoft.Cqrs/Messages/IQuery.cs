namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Queries are used to retrieve data from the system.
/// The handlers should be read only and not alter any state.
/// </summary>
/// <remarks>
/// Names should be in the form of a verb, eg GetOrder, GetRecentBlogPosts
/// </remarks>
public interface IQuery<TResponse> : IMessage<TResponse>;

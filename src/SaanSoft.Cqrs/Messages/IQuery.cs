namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Queries are used to retrieve data from the system
/// </summary>
/// <remarks>
/// Names should be in the form of a verb, eg GetOrder, GetRecentBlogPosts
/// </remarks>
public interface IQuery<TResult> : IMessage<TResult>;

namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// You should never directly inherit from this class
/// use <see cref="IQuery{TQuery, TResult}"/> instead
/// </summary>
public interface IQuery : IMessage
{
}

public interface IQuery<TQuery, TResult> : IQuery
    where TQuery : IQuery<TQuery, TResult>
    where TResult : IQueryResult
{
}

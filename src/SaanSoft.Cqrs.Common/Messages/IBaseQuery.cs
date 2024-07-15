namespace SaanSoft.Cqrs.Common.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseQuery{TMessageId,TQuery,TResponse}"/> instead
/// </summary>
public interface IBaseQuery : IBaseMessage
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseQuery{TMessageId,TQuery,TResponse}"/> instead
/// </summary>
public interface IBaseQuery<TMessageId> : IBaseMessage<TMessageId>, IBaseQuery
    where TMessageId : struct
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseQuery{TMessageId,TQuery,TResponse}"/> instead
/// </summary>
public interface IBaseQuery<TQuery, TResponse> : IBaseQuery
    where TQuery : IBaseQuery<TQuery, TResponse>
{
}

public interface IBaseQuery<TMessageId, TQuery, TResponse> :
    IBaseQuery<TQuery, TResponse>,
    IBaseQuery<TMessageId>
    where TMessageId : struct
    where TQuery : IBaseQuery<TMessageId, TQuery, TResponse>
{
}

namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IQuery{TQuery, TResponse}"/> instead
/// </summary>
public interface IQuery : IMessage
{
}

public interface IQuery<TQuery, TResponse> : IQuery
    where TQuery : IQuery<TQuery, TResponse>
{
}

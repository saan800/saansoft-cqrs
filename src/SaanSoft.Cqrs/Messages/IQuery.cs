namespace SaanSoft.Cqrs.Messages;

// /// <summary>
// /// You should never directly inherit from this interface
// /// use <see cref="IQuery{TMessageId, TQuery, TResult}"/> instead
// /// </summary>
// public interface IQuery : IMessage
// {
// }
//
// /// <summary>
// /// You should never directly inherit from this interface
// /// use <see cref="IQuery{TMessageId, TQuery, TResult}"/> instead
// /// </summary>
// public interface IQuery<TQuery, TResult> : IQuery
//     where TQuery : IQuery<TQuery, TResult>
// {
// }
//
// /// <summary>
// /// You should never directly inherit from this interface
// /// use <see cref="IQuery{TMessageId, TQuery, TResult}"/> instead
// /// </summary>
// public interface IQuery<TMessageId> : IMessage<TMessageId> where TMessageId : struct
// {
// }
//
// public interface IQuery<TMessageId, TQuery, TResult> : IQuery<TQuery, TResult>, IQuery<TMessageId>
//     where TMessageId : struct
//     where TQuery : IQuery<TMessageId, TQuery, TResult>
// {
// }



public interface IQuery : IMessage
{
}


public interface IQuery<TQuery, TResponse> : IMessage<Guid>, IQuery
    where TQuery : IQuery<TQuery, TResponse>
{
}

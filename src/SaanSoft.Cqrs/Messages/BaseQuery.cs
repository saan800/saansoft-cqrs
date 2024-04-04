namespace SaanSoft.Cqrs.Messages;


public abstract class BaseQuery : BaseMessage<Guid>, IQuery
{
    protected BaseQuery(Guid id, Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId)
    {
    }

    protected BaseQuery(Guid id, IMessage<Guid> triggeredByMessage)
        : base(id, triggeredByMessage)
    {
    }
}

public abstract class BaseQuery<TQuery, TResult> : BaseQuery, IQuery<TQuery, TResult>
    where TQuery : IQuery<TQuery, TResult>
{
    protected BaseQuery(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId)
    {
    }

    protected BaseQuery(IMessage<Guid> triggeredByMessage) : base(Guid.NewGuid(), triggeredByMessage)
    {
    }
}


// public abstract class BaseQuery<TQuery, TResponse> : BaseQuery<Guid, TQuery, TResponse>, IQuery<TQuery, TResponse>
//     where TQuery : IQuery<TQuery, TResponse>
// {
// }
//
// public abstract class BaseQuery<TQuery, TResponse> : BaseQuery<Guid, TQuery, TResponse>
//     where TQuery : IQuery<Guid, TQuery, TResponse>
//     where TResponse : IQueryResult
// {

//     protected BaseQuery(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
//         : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }
//
//     protected BaseQuery(IMessage<Guid> triggeredByMessage)
//         : base(Guid.NewGuid(), triggeredByMessage) { }
// }
//
// public abstract class BaseQuery<TMessageId, TQuery, TResponse> : BaseMessage<TMessageId>, IQuery<TQuery, TResponse>
//     where TMessageId : struct
//     where TQuery : IQuery<TQuery, TResponse>
//
// // public abstract class BaseQuery<TMessageId, TQuery, TResponse>
// //     : BaseMessage<TMessageId>, IQuery<TMessageId, TQuery, TResponse>
// //     where TMessageId : struct
// //     where TQuery : IQuery<TMessageId, TQuery, TResponse>
// //     where TResponse : IQueryResult
// {
//     protected BaseQuery(TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
//         : base(id, triggeredById, correlationId, authenticatedId) { }
//
//     protected BaseQuery(TMessageId id, IMessage<TMessageId> triggeredByMessage)
//         : base(id, triggeredByMessage) { }
// }

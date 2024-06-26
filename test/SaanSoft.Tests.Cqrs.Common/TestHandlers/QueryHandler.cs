namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class QueryHandler :
    IQueryHandler<MyQuery, MyQueryResponse>,
    IQueryHandler<AnotherQuery, MyQueryResponse>
{
    public Task<MyQueryResponse> HandleAsync(MyQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(new MyQueryResponse { Message = query.Message });

    public Task<MyQueryResponse> HandleAsync(AnotherQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(new MyQueryResponse { Message = query.Message });
}

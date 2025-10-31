using SaanSoft.Cqrs.Handlers;

namespace SaanSoft.Tests.Cqrs.Shared.TestHandlers;

public class QueryHandler :
    IQueryHandler<MyQuery, MyQueryResponse?>,
    IQueryHandler<AnotherQuery, MyQueryResponse>
{
    public Task<MyQueryResponse?> HandleAsync(MyQuery query, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(query.Message)
            ? null
            : new MyQueryResponse { Message = query.Message };

        return Task.FromResult(result);
    }

    public Task<MyQueryResponse> HandleAsync(AnotherQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(new MyQueryResponse());
}

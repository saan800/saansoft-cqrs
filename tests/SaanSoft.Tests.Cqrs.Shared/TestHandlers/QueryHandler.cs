using SaanSoft.Cqrs.Handlers;

namespace SaanSoft.Tests.Cqrs.Shared.TestHandlers;

public class QueryHandler :
    IQueryHandler<MyQuery, MyQueryResult?>,
    IQueryHandler<AnotherQuery, MyQueryResult>
{
    public Task<MyQueryResult?> HandleAsync(MyQuery query, CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrWhiteSpace(query.Message)
            ? null
            : new MyQueryResult { Message = query.Message };

        return Task.FromResult(result);
    }

    public Task<MyQueryResult> HandleAsync(AnotherQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(new MyQueryResult());
}

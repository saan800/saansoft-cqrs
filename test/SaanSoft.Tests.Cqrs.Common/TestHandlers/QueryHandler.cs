using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Tests.Cqrs.Common.TestModels;
using QueryResponse = SaanSoft.Tests.Cqrs.Common.TestModels.QueryResponse;

namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class QueryHandler :
    IQueryHandler<MyQuery, QueryResponse>,
    IQueryHandler<AnotherQuery, QueryResponse>
{
    public Task<QueryResponse> HandleAsync(IQuery<MyQuery, QueryResponse> query, CancellationToken cancellationToken = default)
        => Task.FromResult(new QueryResponse());

    public Task<QueryResponse> HandleAsync(IQuery<AnotherQuery, QueryResponse> query, CancellationToken cancellationToken = default)
        => Task.FromResult(new QueryResponse());
}

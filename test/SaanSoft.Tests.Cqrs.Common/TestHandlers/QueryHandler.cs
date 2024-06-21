using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Tests.Cqrs.Common.TestModels;

namespace SaanSoft.Tests.Cqrs.Common.TestHandlers;

public class QueryHandler :
    IQueryHandler<MyQuery, MyQueryResponse>,
    IQueryHandler<AnotherQuery, MyQueryResponse>
{
    public Task<MyQueryResponse> HandleAsync(IQuery<MyQuery, MyQueryResponse> query, CancellationToken cancellationToken = default)
        => Task.FromResult(new MyQueryResponse());

    public Task<MyQueryResponse> HandleAsync(IQuery<AnotherQuery, MyQueryResponse> query, CancellationToken cancellationToken = default)
        => Task.FromResult(new MyQueryResponse());
}

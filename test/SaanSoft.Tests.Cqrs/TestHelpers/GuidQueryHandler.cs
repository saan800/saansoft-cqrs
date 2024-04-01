using SaanSoft.Cqrs.Handler;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class GuidQueryHandler : IQueryHandler<GuidQuery, QueryResult>
{
    public Task<QueryResult> HandleAsync(GuidQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new QueryResult
        {
            Message = "Hi from GuidQueryHandler"
        });
    }
}

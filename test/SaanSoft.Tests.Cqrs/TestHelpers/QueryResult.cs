using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class QueryResult : BaseQueryResult
{
    public QueryResult() : base() { }

    public QueryResult(string errorMessage) : base(errorMessage) { }

    public string Message { get; set; } = string.Empty;
}

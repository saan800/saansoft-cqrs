using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class QueryResponse : BaseQueryResponse
{
    public QueryResponse() { }

    public QueryResponse(string errorMessage) : base(errorMessage) { }

    public string SomeData { get; set; } = string.Empty;
}

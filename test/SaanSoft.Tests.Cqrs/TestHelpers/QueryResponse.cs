namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class QueryResponse : SaanSoft.Cqrs.Messages.QueryResponse
{
    public QueryResponse() { }

    public QueryResponse(string errorMessage) : base(errorMessage) { }

    public string SomeData { get; set; } = string.Empty;
}

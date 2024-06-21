namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyQueryResponse : SaanSoft.Cqrs.Messages.QueryResponse
{
    public MyQueryResponse() { }

    public MyQueryResponse(string errorMessage) : base(errorMessage) { }

    public string SomeData { get; set; } = string.Empty;
}

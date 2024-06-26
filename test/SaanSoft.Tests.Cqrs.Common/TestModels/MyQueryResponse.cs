namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyQueryResponse
{
    public MyQueryResponse()
    {
    }

    public MyQueryResponse(string data)
    {
        SomeData = data;
    }

    public string? SomeData { get; set; }
}

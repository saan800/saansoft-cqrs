namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyQueryResponse
{
    public MyQueryResponse()
    {
    }

    public MyQueryResponse(string message)
    {
        Message = message;
    }

    public string? Message { get; set; }
}

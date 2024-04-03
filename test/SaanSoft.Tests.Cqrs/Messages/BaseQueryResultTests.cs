using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Messages;

public class BaseQueryResultTests
{
    [Fact]
    public void Empty_constructor_is_successful_result()
    {
        var result = new QueryResult();
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [AutoFakeData]
    public void Constructor_with_error_message_is_failure_result(string errorMessage)
    {
        var result = new QueryResult(errorMessage);
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }
}

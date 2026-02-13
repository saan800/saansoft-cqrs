using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Tests.Cqrs.Transport;

public class ExternalResponseTests
{
    [Fact]
    public void FromSuccess()
    {
        var result = ExternalResponse.FromSuccess();
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Exception.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Hello world")]
    public void FromSuccess_with_payload(string? payload)
    {
        var result = ExternalResponse<string>.FromSuccess(payload);
        result.Success.Should().BeTrue();
        result.Payload.Should().Be(payload);
        result.ErrorMessage.Should().BeNull();
        result.Exception.Should().BeNull();
    }

    [Theory]
    [InlineData("oops")]
    public void FromError(string errorMessage)
    {
        var result = ExternalResponse<string>.FromError(errorMessage);
        result.Success.Should().BeFalse();
        result.Payload.Should().BeNull();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Exception.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void FromException(Exception exception)
    {
        var result = ExternalResponse<string>.FromException(exception);
        result.Success.Should().BeFalse();
        result.Payload.Should().BeNull();
        result.ErrorMessage.Should().Be(exception.Message);
        result.Exception.Should().Be(exception);
    }

    [Theory]
    [AutoFakeData]
    public void FromError_with_message_and_exception(string errorMessage, Exception exception)
    {
        var result = ExternalResponse<string>.FromError(errorMessage, exception);
        result.Success.Should().BeFalse();
        result.Payload.Should().BeNull();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Exception.Should().Be(exception);
    }
}

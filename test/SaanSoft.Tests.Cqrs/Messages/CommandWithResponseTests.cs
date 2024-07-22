namespace SaanSoft.Tests.Cqrs.Messages;

public class CommandWithResponseTests
{
    [Theory]
    [InlineAutoData]
    public void Init_populates_properties_with_defaults(string message)
    {
        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse { Message = message };
        result.Id.Should().NotBe(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.Message.Should().Be(message);
        result.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.CorrelationId.Should().BeNull();
        result.TriggeredByUser.Should().BeNull();
        result.Metadata.TriggeredByMessageId.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(string correlationId, string authId, string message)
    {
        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse(correlationId, authId) { Message = message };
        result.Id.Should().NotBe(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.Message.Should().Be(message);
        result.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.CorrelationId.Should().Be(correlationId);
        result.TriggeredByUser.Should().Be(authId);
        result.Metadata.TriggeredByMessageId.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(Guid id, string correlationId, string authId, string message)
    {
        var triggeredBy = new MyCommand(correlationId, authId) { Id = id };

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse(triggeredBy) { Message = message };
        result.Id.Should().NotBe(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.Message.Should().Be(message);
        result.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.TriggeredByUser.Should().Be(triggeredBy.TriggeredByUser);
        result.Metadata.TriggeredByMessageId.Should().Be(triggeredBy.Id.ToString());
    }
}

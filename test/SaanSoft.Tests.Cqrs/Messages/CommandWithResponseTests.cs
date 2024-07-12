namespace SaanSoft.Tests.Cqrs.Messages;

public class CommandWithResponseTests
{
    [Theory]
    [InlineAutoData]
    public void Init_populates_properties_with_defaults(string message)
    {
        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse { Message = message };
        result.Id.Should().Be(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.Message.Should().Be(message);
        result.Metadata.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.Metadata.TriggeredById.Should().BeNull();
        result.Metadata.CorrelationId.Should().BeNull();
        result.Metadata.TriggeredByUser.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(string correlationId, string authId, string message)
    {
        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse(correlationId, authId) { Message = message };
        result.Id.Should().Be(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.Message.Should().Be(message);
        result.Metadata.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.Metadata.TriggeredById.Should().BeNull();
        result.Metadata.CorrelationId.Should().Be(correlationId);
        result.Metadata.TriggeredByUser.Should().Be(authId);
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(Guid id, string correlationId, string authId, string message)
    {
        var triggeredBy = new MyCommand(correlationId, authId) { Id = id };

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse(triggeredBy) { Message = message };
        result.Id.Should().Be(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.Message.Should().Be(message);
        result.Metadata.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.Metadata.TriggeredById.Should().Be(triggeredBy.Id.ToString());
        result.Metadata.CorrelationId.Should().Be(triggeredBy.Metadata.CorrelationId);
        result.Metadata.TriggeredByUser.Should().Be(triggeredBy.Metadata.TriggeredByUser);
    }
}

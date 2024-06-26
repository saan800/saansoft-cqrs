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
        result.CorrelationId.Should().BeNull();
        result.TriggeredByUser.Should().BeNull();
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TriggeredById.Should().BeNull();
        result.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.Message.Should().Be(message);
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(Guid id, string correlationId, string authId, string message)
    {
        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse(id, correlationId, authId) { Message = message };
        result.Id.Should().Be(id);
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().Be(correlationId);
        result.TriggeredByUser.Should().Be(authId);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TriggeredById.Should().BeNull();
        result.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.Message.Should().Be(message);
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(Guid id, string correlationId, string authId, string message)
    {
        var triggeredBy = new MyCommand(id, correlationId, authId);

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;

        var result = new MyCommandWithResponse(triggeredBy) { Message = message };
        result.Id.Should().Be(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.TriggeredByUser.Should().Be(triggeredBy.TriggeredByUser);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.TriggeredById.Should().Be(triggeredBy.Id);
        result.TypeFullName.Should().Be(typeof(MyCommandWithResponse).FullName);
        result.Message.Should().Be(message);
    }
}

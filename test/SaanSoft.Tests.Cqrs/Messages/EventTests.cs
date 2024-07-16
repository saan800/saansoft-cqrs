namespace SaanSoft.Tests.Cqrs.Messages;

public class EventTests
{
    [Fact]
    public void Init_populates_properties_with_defaults()
    {
        var startTime = DateTime.UtcNow;
        var key = Guid.NewGuid();
        var result = new MyEvent(key);

        result.Key.Should().Be(key);
        result.Id.Should().Be(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        result.CorrelationId.Should().BeNull();
        result.TriggeredByUser.Should().BeNull();
        result.Metadata.TriggeredByMessageId.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(Guid key, string correlationId, string authId)
    {
        var startTime = DateTime.UtcNow;
        var result = new MyEvent(key, correlationId, authId);

        result.Key.Should().Be(key);
        result.Id.Should().Be(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        result.CorrelationId.Should().Be(correlationId);
        result.TriggeredByUser.Should().Be(authId);
        result.Metadata.TriggeredByMessageId.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(Guid id, Guid key, string correlationId, string authId)
    {
        var triggeredBy = new MyCommand(correlationId, authId) { Id = id };

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;
        var result = new MyEvent(key, triggeredBy);
        result.Key.Should().Be(key);
        result.Id.Should().Be(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.TriggeredByUser.Should().Be(triggeredBy.TriggeredByUser);
        result.Metadata.TriggeredByMessageId.Should().Be(triggeredBy.Id.ToString());
    }
}

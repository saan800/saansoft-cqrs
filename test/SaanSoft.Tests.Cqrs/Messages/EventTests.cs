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
        result.Metadata.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        result.Metadata.CorrelationId.Should().BeNull();
        result.Metadata.TriggeredByUser.Should().BeNull();
        result.Metadata.TriggeredById.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(Guid key, Guid id, string correlationId, string authId)
    {
        var startTime = DateTime.UtcNow;
        var result = new MyEvent(key, id, correlationId, authId);

        result.Key.Should().Be(key);
        result.Id.Should().Be(id);
        result.Id.Should().NotBe(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.Metadata.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        result.Metadata.TriggeredById.Should().BeNull();
        result.Metadata.CorrelationId.Should().Be(correlationId);
        result.Metadata.TriggeredByUser.Should().Be(authId);
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(Guid id, Guid key, string correlationId, string authId)
    {
        var triggeredBy = new MyCommand(id, correlationId, authId);

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;
        var result = new MyEvent(key, triggeredBy);
        result.Key.Should().Be(key);
        result.Id.Should().Be(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.Metadata.TypeFullName.Should().Be(typeof(MyEvent).FullName);
        result.Metadata.TriggeredById.Should().Be(triggeredBy.Id.ToString());
        result.Metadata.CorrelationId.Should().Be(triggeredBy.Metadata.CorrelationId);
        result.Metadata.TriggeredByUser.Should().Be(triggeredBy.Metadata.TriggeredByUser);
    }
}

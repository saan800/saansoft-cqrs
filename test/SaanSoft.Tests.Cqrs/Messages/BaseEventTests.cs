using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Messages;

public class BaseEventTests
{
    [Fact]
    public void Init_populates_properties_with_defaults()
    {
        var startTime = DateTime.UtcNow;
        var key = Guid.NewGuid();
        var result = new GuidEvent(key);

        result.Key.Should().Be(key);
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().BeNull();
        result.AuthenticatedId.Should().BeNull();
        result.ReceivedOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TypeFullName.Should().Be(typeof(GuidEvent).FullName);
        result.TriggeredById.Should().Be(default(Guid)); // :( TODO: Figure out how to make this null
    }

    [Fact]
    public void Init_populates_properties_from_constructor()
    {
        var startTime = DateTime.UtcNow;
        var key = Guid.NewGuid();
        var triggeredById = Guid.NewGuid();
        var correlationId = Guid.NewGuid().ToString();
        var authId = "someone";
        var result = new GuidEvent(key, triggeredById, correlationId, authId);

        result.Key.Should().Be(key);
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().Be(correlationId);
        result.AuthenticatedId.Should().Be(authId);
        result.ReceivedOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TypeFullName.Should().Be(typeof(GuidEvent).FullName);
        result.TriggeredById.Should().Be(triggeredById);
    }

    [Fact]
    public void Init_populates_properties_from_triggerMessage()
    {
        var key = Guid.NewGuid();
        var correlationId = Guid.NewGuid().ToString();
        var authId = "someone";
        var triggeredBy = new GuidCommand(correlationId, authId);

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;
        var result = new GuidEvent(key, triggeredBy);
        result.Key.Should().Be(key);
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.AuthenticatedId.Should().Be(triggeredBy.AuthenticatedId);
        result.ReceivedOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.ReceivedOnUtc.Should().NotBe(triggeredBy.ReceivedOnUtc);
        result.TriggeredById.Should().Be(triggeredBy.Id);
        result.TypeFullName.Should().Be(typeof(GuidEvent).FullName);
    }
}

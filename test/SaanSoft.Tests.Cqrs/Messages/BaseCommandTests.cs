using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Messages;

public class BaseCommandTests
{
    [Fact]
    public void Init_populates_properties_with_defaults()
    {
        var startTime = DateTime.UtcNow;

        var result = new GuidCommand();
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().BeNull();
        result.AuthenticatedId.Should().BeNull();
        result.ReceivedOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TriggeredById.Should().Be(default(Guid)); // :( TODO: Figure out how to make this null
        result.TypeFullName.Should().Be(typeof(GuidCommand).FullName);
    }

    [Fact]
    public void Init_populates_properties_from_constructor()
    {
        var startTime = DateTime.UtcNow;
        var triggeredById = Guid.NewGuid();
        var correlationId = Guid.NewGuid().ToString();
        var authId = "someone";
        var result = new GuidCommand(triggeredById, correlationId, authId);

        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().Be(correlationId);
        result.AuthenticatedId.Should().Be(authId);
        result.ReceivedOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TriggeredById.Should().Be(triggeredById);
        result.TypeFullName.Should().Be(typeof(GuidCommand).FullName);
    }

    [Fact]
    public void Init_populates_properties_from_triggerMessage()
    {
        var correlationId = Guid.NewGuid().ToString();
        var authId = "someone";
        var triggeredBy = new GuidCommand(correlationId, authId);

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;

        var result = new GuidCommand(triggeredBy);
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.AuthenticatedId.Should().Be(triggeredBy.AuthenticatedId);
        result.ReceivedOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.ReceivedOnUtc.Should().NotBe(triggeredBy.ReceivedOnUtc);
        result.TriggeredById.Should().Be(triggeredBy.Id);
        result.TypeFullName.Should().Be(typeof(GuidCommand).FullName);
    }
}

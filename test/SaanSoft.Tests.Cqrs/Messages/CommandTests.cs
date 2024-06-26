namespace SaanSoft.Tests.Cqrs.Messages;

public class CommandTests
{
    [Fact]
    public void Init_populates_properties_with_defaults()
    {
        var startTime = DateTime.UtcNow;

        var result = new MyCommand();
        result.Id.Should().Be(default(Guid));
        result.CorrelationId.Should().BeNull();
        result.TriggeredByUser.Should().BeNull();
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TriggeredById.Should().BeNull();
        result.TypeFullName.Should().Be(typeof(MyCommand).FullName);
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(Guid id, string correlationId, string authId)
    {
        var startTime = DateTime.UtcNow;

        var result = new MyCommand(id, correlationId, authId);

        result.Id.Should().Be(id);
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().Be(correlationId);
        result.TriggeredByUser.Should().Be(authId);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TriggeredById.Should().BeNull();
        result.TypeFullName.Should().Be(typeof(MyCommand).FullName);
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(Guid id, string correlationId, string authId)
    {
        var triggeredBy = new MyCommand(id, correlationId, authId);

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;

        var result = new MyCommand(triggeredBy);
        result.Id.Should().Be(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.TriggeredByUser.Should().Be(triggeredBy.TriggeredByUser);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.TriggeredById.Should().Be(triggeredBy.Id);
        result.TypeFullName.Should().Be(typeof(MyCommand).FullName);
    }
}

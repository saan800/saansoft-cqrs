namespace SaanSoft.Tests.Cqrs.Messages;

public class QueryTests
{
    [Fact]
    public void Init_populates_properties_with_defaults()
    {
        var startTime = DateTime.UtcNow;
        var result = new MyQuery();
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().BeNull();
        result.AuthenticationId.Should().BeNull();
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        result.TriggeredById.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(string correlationId, string authId)
    {
        var startTime = DateTime.UtcNow;
        var result = new MyQuery(correlationId, authId);

        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.CorrelationId.Should().Be(correlationId);
        result.AuthenticationId.Should().Be(authId);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TriggeredById.Should().BeNull();
        result.TypeFullName.Should().Be(typeof(MyQuery).FullName);
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(string correlationId, string authId)
    {
        var triggeredBy = new MyCommand(correlationId, authId);

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;

        var result = new MyQuery(triggeredBy);
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.AuthenticationId.Should().Be(triggeredBy.AuthenticationId);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.TriggeredById.Should().Be(triggeredBy.Id);
        result.TypeFullName.Should().Be(typeof(MyQuery).FullName);
    }
}

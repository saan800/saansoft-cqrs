namespace SaanSoft.Tests.Cqrs.Messages;

public class QueryTests
{
    [Fact]
    public void Init_populates_properties_with_defaults()
    {
        var startTime = DateTime.UtcNow;
        var result = new MyQuery();
        result.Id.Should().NotBe(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        result.CorrelationId.Should().BeNull();
        result.TriggeredByUser.Should().BeNull();
        result.Metadata.TriggeredByMessageId.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_constructor(string correlationId, string authId)
    {
        var startTime = DateTime.UtcNow;
        var result = new MyQuery(correlationId, authId);

        result.Id.Should().NotBe(default(Guid));
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        result.CorrelationId.Should().Be(correlationId);
        result.TriggeredByUser.Should().Be(authId);
        result.Metadata.TriggeredByMessageId.Should().BeNull();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(Guid id, string correlationId, string authId)
    {
        var triggeredBy = new MyCommand(correlationId, authId) { Id = id };

        Thread.Sleep(50);

        var startTime = DateTime.UtcNow;

        var result = new MyQuery(triggeredBy);
        result.Id.Should().NotBe(default(Guid));
        result.Id.Should().NotBe(triggeredBy.Id);
        result.MessageOnUtc.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.MessageOnUtc.Should().NotBe(triggeredBy.MessageOnUtc);
        result.TypeFullName.Should().Be(typeof(MyQuery).FullName);
        result.CorrelationId.Should().Be(triggeredBy.CorrelationId);
        result.TriggeredByUser.Should().Be(triggeredBy.TriggeredByUser);
        result.Metadata.TriggeredByMessageId.Should().Be(triggeredBy.Id.ToString());
    }
}

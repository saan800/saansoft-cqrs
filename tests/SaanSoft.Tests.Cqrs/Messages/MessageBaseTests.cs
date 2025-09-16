namespace SaanSoft.Tests.Cqrs.Messages;

public class MessageBaseTests
{
    [Fact]
    public void Init_populates_properties_with_defaults()
    {
        var startTime = DateTime.UtcNow;

        var message = new AMessage();
        message.Id.Should().NotBe(Guid.Empty);
        message.Id.Should().NotBe(default(Guid));
        message.OccurredOn.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        message.CorrelationId.Should().BeNull();
        message.AuthenticationId.Should().BeNull();
        message.TriggeredByMessageId.Should().BeNull();
        message.IsReplay.Should().BeFalse();
    }

    [Theory]
    [AutoFakeData]
    public void Init_populates_properties_from_triggerMessage(AMessage triggeredByMessage)
    {
        triggeredByMessage.OccurredOn = DateTime.UtcNow;

        Thread.Sleep(50);

        var newMessage = new AMessage(triggeredByMessage);

        newMessage.Id.Should().NotBe(Guid.Empty);
        newMessage.Id.Should().NotBe(default(Guid));
        newMessage.Id.Should().NotBe(triggeredByMessage.Id);
        newMessage.OccurredOn.Should().NotBe(triggeredByMessage.OccurredOn);
        newMessage.OccurredOn.Should().BeOnOrAfter(triggeredByMessage.OccurredOn).And.BeOnOrBefore(DateTime.UtcNow);
        newMessage.TriggeredByMessageId.Should().Be(triggeredByMessage.Id);
        newMessage.CorrelationId.Should().Be(triggeredByMessage.CorrelationId);
        newMessage.AuthenticationId.Should().Be(triggeredByMessage.AuthenticationId);
        newMessage.IsReplay.Should().Be(triggeredByMessage.IsReplay);
    }
}

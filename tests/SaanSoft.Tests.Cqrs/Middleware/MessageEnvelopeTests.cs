using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Middleware;

public class MessageEnvelopeTests
{
    [Fact]
    public void Wrap_throws_exception_if_message_is_null()
    {
        Action act = () => MessageEnvelope.Wrap((IMessage)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoFakeData]
    public void Wrap_instantiates_from_message(AMessage message)
    {
        var result = MessageEnvelope.Wrap(message);

        result.Id.Should().Be(message.Id);
        result.OccurredOn.Should().Be(message.OccurredOn);

        result.MessageType.Should().Be(message.GetType().GetTypeFullName());
        result.Message.Should().Be(message);

        result.Publisher.Should().BeNull();
        result.Handlers.Should().BeEmpty();
        result.Metadata.Should().BeEmpty();
    }

    [Fact]
    public void Wrap_ensures_message_and_envelope_id_is_not_default()
    {
        var message = new AMessage { Id = Guid.Empty };
        var result = MessageEnvelope.Wrap(message);

        message.Id.Should().NotBe(Guid.Empty);
        result.Id.Should().NotBe(Guid.Empty);

        result.Id.Should().Be(message.Id);
    }

    [Fact]
    public void Wrap_ensures_message_and_envelope_OccurredOn_is_not_default()
    {
        var startTime = DateTime.UtcNow;

        var defaultDateTime = DateTime.MinValue;
        var message = new AMessage { OccurredOn = defaultDateTime };
        var result = MessageEnvelope.Wrap(message);

        message.OccurredOn.Should().NotBe(defaultDateTime);
        result.OccurredOn.Should().NotBe(defaultDateTime);

        message.OccurredOn.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);
        result.OccurredOn.Should().BeOnOrAfter(startTime).And.BeOnOrBefore(DateTime.UtcNow);

        result.OccurredOn.Should().Be(message.OccurredOn);
    }

    [Fact]
    public void MarkPending_adds_one_pending_handler()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkPending(handlerName);

        envelope.Handlers.Should().HaveCount(1);

        var handlerRecord = envelope.Handlers.First();
        handlerRecord.HandlerName.Should().Be(handlerName);
        handlerRecord.Status.Should().Be(HandlerStatus.Pending);
        handlerRecord.HandledOnUtc.Should().BeNull();
        handlerRecord.ErrorMessage.Should().BeNull();
        handlerRecord.Exception.Should().BeNull();
    }

    [Fact]
    public void MarkPending_multiple_calls_for_same_handler_only_adds_one_pending_handler()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkPending(handlerName);
        envelope.MarkPending(handlerName);
        envelope.MarkPending(handlerName);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Pending);
    }

    [Fact]
    public void MarkPending_multiple_calls_for_different_handlers_adds_one_pending_handler_each()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName1 = "SomeHandler";
        var handlerName2 = "AnotherHandler";
        envelope.MarkPending(handlerName1);
        envelope.MarkPending(handlerName1);
        envelope.MarkPending(handlerName2);

        envelope.Handlers.Should().HaveCount(2);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName1 && h.Status == HandlerStatus.Pending);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName2 && h.Status == HandlerStatus.Pending);
    }

    [Fact]
    public void MarkPending_with_existing_errored_handler_then_pending_handler_only_adds_one_pending_handler()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkFailed(handlerName, "oops");
        envelope.MarkPending(handlerName);
        envelope.MarkPending(handlerName);

        envelope.Handlers.Should().HaveCount(2);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Pending);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Failed);
    }

    [Fact]
    public void MarkPending_with_existing_succeeded_handler_then_pending_handler_only_adds_one_pending_handler()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkSuccess(handlerName);
        envelope.MarkPending(handlerName);
        envelope.MarkPending(handlerName);

        envelope.Handlers.Should().HaveCount(2);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Pending);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Success);
    }

    [Fact]
    public void MarkSuccess_adds_one_succeeded_handler_record()
    {
        var startTime = DateTime.UtcNow;
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkSuccess(handlerName);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h =>
                h.HandlerName == handlerName &&
                h.Status == HandlerStatus.Success &&
                h.HandledOnUtc >= startTime &&
                h.HandledOnUtc <= DateTime.UtcNow &&
                h.ErrorMessage == null &&
                h.Exception == null
            );
    }

    [Fact]
    public void MarkSuccess_multiple_calls_for_same_handler_adds_record_per_call()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkSuccess(handlerName);
        envelope.MarkSuccess(handlerName);
        envelope.MarkSuccess(handlerName);

        envelope.Handlers.Should().HaveCount(3);
    }

    [Fact]
    public void MarkSuccess_updates_existing_pending_record()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkPending(handlerName);
        envelope.MarkSuccess(handlerName);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Success);
    }

    [Fact]
    public void MarkSuccess_updates_existing_failed_record()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkFailed(handlerName, "oops");
        envelope.MarkSuccess(handlerName);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Success);
    }

    [Fact]
    public void MarkSuccess_simplifies_multiple_pending_and_failed_records_to_only_success()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        envelope.MarkFailed(handlerName, "oops");
        envelope.MarkFailed(handlerName, "oops 2");
        envelope.MarkPending(handlerName);
        envelope.MarkSuccess(handlerName);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Success);
    }

    [Fact]
    public void MarkFailed_adds_one_failed_handler_record()
    {
        var startTime = DateTime.UtcNow;
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        var errorMessage = "oops";
        envelope.MarkFailed(handlerName, errorMessage);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h =>
                h.HandlerName == handlerName &&
                h.Status == HandlerStatus.Failed &&
                h.HandledOnUtc >= startTime &&
                h.HandledOnUtc <= DateTime.UtcNow &&
                h.ErrorMessage == errorMessage &&
                h.Exception == null
            );
    }

    [Fact]
    public void MarkFailed_with_exception_adds_one_failed_handler_record()
    {
        var startTime = DateTime.UtcNow;
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        var exception = new Exception("oops");
        envelope.MarkFailed(handlerName, exception);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h =>
                h.HandlerName == handlerName &&
                h.Status == HandlerStatus.Failed &&
                h.HandledOnUtc >= startTime &&
                h.HandledOnUtc <= DateTime.UtcNow &&
                h.ErrorMessage == exception.Message &&
                h.Exception == exception
            );
    }


    [Fact]
    public void MarkFailed_multiple_calls_for_same_handler_adds_failed_record_per_call()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        var errorMessage = "oops";
        envelope.MarkFailed(handlerName, errorMessage);
        envelope.MarkFailed(handlerName, errorMessage);
        envelope.MarkFailed(handlerName, errorMessage);

        envelope.Handlers.Should().HaveCount(3);
    }

    [Fact]
    public void MarkFailed_updates_existing_pending_record()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        var errorMessage = "oops";
        envelope.MarkPending(handlerName);
        envelope.MarkPending(handlerName);
        envelope.MarkFailed(handlerName, errorMessage);

        envelope.Handlers.Should().HaveCount(1);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Failed);
    }

    [Fact]
    public void MarkFailed_when_existing_success_record_adds_new_failed_record()
    {
        var envelope = MessageEnvelope.Wrap(new AMessage());
        envelope.Handlers.Should().BeEmpty();

        var handlerName = "SomeHandler";
        var errorMessage = "oops";
        envelope.MarkSuccess(handlerName);
        envelope.MarkFailed(handlerName, errorMessage);

        envelope.Handlers.Should().HaveCount(2);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Success);
        envelope.Handlers.Should().Contain(h => h.HandlerName == handlerName && h.Status == HandlerStatus.Failed);
    }
}

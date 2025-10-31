using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Utilities;

public static class MessageExtensions
{
    private const string MessageIdKey = "MessageId";
    private const string HandlerTypeKey = "HandlerType";

    public static Dictionary<string, object> BuildLoggingScopeData(
        this MessageEnvelope envelope, Type? handlerType = null)
    {
        var scopeData = new Dictionary<string, object>
        {
            [MessageIdKey] = envelope.Id.ToString(),
            [nameof(MessageEnvelope.MessageType)] = envelope.MessageType,
        };


        if (!string.IsNullOrWhiteSpace(envelope.Publisher))
            scopeData.Add(nameof(MessageEnvelope.Publisher), envelope.Publisher);

        var message = (IMessage)envelope.Message;
        if (!string.IsNullOrWhiteSpace(message.CorrelationId))
            scopeData.Add(nameof(IMessage.CorrelationId), message.CorrelationId);

        if (message.IsReplay)
            scopeData.Add(nameof(IMessage.IsReplay), true);

        if (handlerType != null)
            scopeData.Add(HandlerTypeKey, handlerType.GetTypeFullName());

        return scopeData;
    }

    public static Dictionary<string, object> BuildLoggingScopeData(
        this IReadOnlyCollection<MessageEnvelope> envelopes, Type? handlerType = null)
    {
        if (envelopes.Count == 1) return BuildLoggingScopeData(envelopes.Single(), handlerType);

        var scopeData = new Dictionary<string, object>();
        if (envelopes.Count == 0) return scopeData;

        string? messageType = null;
        string? correlationId = null;
        string? publisher = null;
        bool? singleIsReplay = null;

        var multipleMessageTypes = false;
        var multipleCorrelationIds = false;
        var multiplePublishers = false;
        var multipleIsReplay = false;

        foreach (var env in envelopes)
        {
            // MessageType
            if (messageType == null)
                messageType = env.MessageType;
            else if (!multipleMessageTypes && messageType != env.MessageType)
                multipleMessageTypes = true;

            // Publisher
            if (!string.IsNullOrWhiteSpace(env.Publisher))
            {
                if (publisher == null)
                    publisher = env.Publisher;
                else if (!multiplePublishers && publisher != env.Publisher)
                    multiplePublishers = true;
            }

            var message = (IMessage)env.Message;

            // CorrelationId
            if (!string.IsNullOrWhiteSpace(message.CorrelationId))
            {
                if (correlationId == null)
                    correlationId = message.CorrelationId;
                else if (!multipleCorrelationIds && correlationId != message.CorrelationId)
                    multipleCorrelationIds = true;
            }

            // IsReplay
            var isReplay = message.IsReplay;
            if (singleIsReplay == null)
                singleIsReplay = isReplay;
            else if (!multipleIsReplay && singleIsReplay.Value != isReplay)
                multipleIsReplay = true;
        }

        if (!multipleMessageTypes && messageType != null)
            scopeData.Add(nameof(MessageEnvelope.MessageType), messageType);

        if (!multiplePublishers && publisher != null)
            scopeData.Add(nameof(MessageEnvelope.Publisher), publisher);

        if (!multipleCorrelationIds && correlationId != null)
            scopeData.Add(nameof(IMessage.CorrelationId), correlationId);

        if (!multipleIsReplay && singleIsReplay == true)
            scopeData.Add(nameof(IMessage.IsReplay), true);

        if (handlerType != null)
            scopeData.Add(HandlerTypeKey, handlerType.GetTypeFullName());

        return scopeData;
    }
}

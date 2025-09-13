namespace SaanSoft.Cqrs.Utilities;

public static class MessageExtensions
{
    public static Dictionary<string, object> BuildLoggingScopeData<TMessage>(this TMessage message, Type? handlerType = null)
        where TMessage : IMessage
    {
        var scopeData = new Dictionary<string, object>
        {
            ["MessageType"] = message.GetType().GetTypeFullName()
        };

#pragma warning disable CS8601 // Possible null reference assignment.
        if (!GenericUtils.IsNullOrDefault(message.Id))
            scopeData.Add("MessageId", message.Id!.ToString());
#pragma warning restore CS8601 // Possible null reference assignment.

        if (!string.IsNullOrWhiteSpace(message.CorrelationId))
            scopeData.Add(nameof(message.CorrelationId), message.CorrelationId);

        if (handlerType != null)
            scopeData.Add("HandlerType", handlerType.GetTypeFullName());

        if (message.IsReplay)
            scopeData.Add(nameof(message.CorrelationId), true);

        return scopeData;
    }
}

namespace SaanSoft.Cqrs.Decorator.LoggerScope;

public static class MessageExtensions
{
    public static Dictionary<string, object> BuildLoggingScopeData<TMessageId>(this IBaseMessage<TMessageId> message, Type? handlerType = null)
        where TMessageId : struct
    {
        var scopeData = new Dictionary<string, object>
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ["MessageId"] = !GenericUtils.IsNullOrDefault(message.Id) ? message.Id!.ToString() : string.Empty,
#pragma warning restore CS8601 // Possible null reference assignment.
            ["MessageType"] = message.Metadata.TypeFullName ?? string.Empty,
            ["CorrelationId"] = message.Metadata.CorrelationId ?? string.Empty,
        };
        if (handlerType != null) scopeData.Add("HandlerType", handlerType.GetTypeFullName());
        if (message.IsReplay) scopeData.Add("IsReplay", true);
        return scopeData;
    }
}

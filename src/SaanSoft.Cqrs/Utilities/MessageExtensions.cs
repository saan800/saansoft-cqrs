namespace SaanSoft.Cqrs.Utilities;

public static class MessageExtensions
{
    public static Dictionary<string, object> BuildLoggingScopeData<TMessageId>(this IMessage<TMessageId> message, Type handlerType)
        where TMessageId : struct
    {
        var scopeData = new Dictionary<string, object>
        {
            ["MessageId"] = !GenericUtils.IsNullOrDefault(message.Id) ? message.Id!.ToString() : string.Empty,
            ["MessageType"] = message.Metadata.TypeFullName ?? string.Empty,
            ["CorrelationId"] = message.Metadata.CorrelationId ?? string.Empty,
            ["HandlerType"] = handlerType.FullName ?? handlerType.Name
        };
        if (message.IsReplay) scopeData.Add("IsReplay", true);
        return scopeData;
    }
}

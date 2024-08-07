namespace SaanSoft.Cqrs.Utilities;

public static class MessageExtensions
{
    public static Dictionary<string, object> BuildLoggingScopeData(this IMessage message, Type? handlerType = null)
    {
        var scopeData = new Dictionary<string, object>
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ["MessageId"] = !GenericUtils.IsNullOrDefault(message.Id) ? message.Id!.ToString() : string.Empty,
#pragma warning restore CS8601 // Possible null reference assignment.
            ["MessageType"] = message.TypeFullName ?? string.Empty,
            ["CorrelationId"] = message.CorrelationId ?? string.Empty,
        };
        if (handlerType != null) scopeData.Add("HandlerType", handlerType.GetTypeFullName());
        if (message.IsReplay) scopeData.Add("IsReplay", true);
        return scopeData;
    }
}

namespace SaanSoft.Cqrs.Messages;

public class MessageMetadata : Dictionary<string, object?>
{
    /// <summary>
    /// Used to track related commands/events/queries.
    /// Should be propagated between related messages.
    ///
    /// The initial message could be populated by services such as Azure AppInsights, OpenTelemetry,
    /// Http header (e.g. "X-Request-Id"), or a simple guid (as string)
    /// </summary>
    public string? CorrelationId
    {
        get => TryGetValueAs<string>(nameof(CorrelationId), out var val) ? val : null;
        set => Add(nameof(CorrelationId), value);
    }

    /// <summary>
    /// Record if this message was triggered by another command/event/query
    /// Should be populated by the initiating command/event/query/message Id
    /// Similar to CorrelationId, it provides a way to track messages through the system
    /// </summary>
    public string? TriggeredById
    {
        get => TryGetValueAs<string>(nameof(TriggeredById), out var val) ? val : null;
        set => Add(nameof(TriggeredById), value);
    }

    /// <summary>
    /// Who triggered the command/event/query (eg UserId, third party (eg Auth0) Id).
    ///
    /// Should be propagated between related messages.
    ///
    /// IMPORTANT: Do not use any PII data.
    /// </summary>
    public string? TriggeredByUser
    {
        get => TryGetValueAs<string>(nameof(TriggeredByUser), out var val) ? val : null;
        set => Add(nameof(TriggeredByUser), value);
    }

    /// <summary>
    /// FullName for the type of the message
    /// </summary>
    public string TypeFullName
    {
        get => TryGetValueAs<string>(nameof(TypeFullName), out var val) ? val! : "";
        set => Add(nameof(TypeFullName), value);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool TryGetValueAs<T>(string key, out T? value)
    {
        var found = TryGetValue(key, out object? obj);
        value = (T?)obj;
        return found;
    }
}

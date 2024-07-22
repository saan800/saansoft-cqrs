namespace SaanSoft.Cqrs.Messages;

public class MessageMetadata : Dictionary<string, object?>
{
    /// <summary>
    /// Record if this message was triggered by another command/event/query
    /// Should be populated by the initiating command/event/query/message Id
    /// Similar to CorrelationId, it provides a way to track messages through the system
    /// </summary>
    public string? TriggeredByMessageId
    {
        get => TryGetValueAs<string>(nameof(TriggeredByMessageId), out var val) ? val : null;
        set => AddOrUpdate(nameof(TriggeredByMessageId), value);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool TryGetValueAs<T>(string key, out T? value)
    {
        var found = TryGetValue(key, out object? obj);
        value = (T?)obj;
        return found;
    }

    public T? GetValueOrDefaultAs<T>(string key)
    {
        return TryGetValueAs(key, out T? value)
            ? value
            : default;
    }

    public void AddOrUpdate(string key, object? value)
    {
        if (value == null)
        {
            Remove(key);
            return;
        }
        this[key] = value;
    }
}

namespace SaanSoft.Cqrs.Utilities;

public static class GenericUtils
{
    public static bool IsNullOrDefault<TMessageId>(TMessageId? value)
    {
        // If value is null, return true
        if (ReferenceEquals(value, null))
            return true;

        // If value is a reference type and equals its default value, return true
        if (EqualityComparer<TMessageId>.Default.Equals(value, default))
            return true;

        // If value is a value type and equals its default value, return true
        if (value.Equals(default(TMessageId)))
            return true;

        // Otherwise, return false
        return false;
    }
}

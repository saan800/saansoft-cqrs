namespace SaanSoft.Cqrs.Utilities;

public static class GenericUtils
{
    public static bool IsNullOrDefault<T>(T? value)
    {
        // If value is null, return true
        if (ReferenceEquals(value, null))
            return true;

        // If value is a reference type and equals its default value, return true
        if (EqualityComparer<T>.Default.Equals(value, default))
            return true;

        // If value is a value type and equals its default value, return true
        if (value.Equals(default(T)) || value.Equals(default(T?)))
            return true;

        // Otherwise, return false
        return false;
    }
}

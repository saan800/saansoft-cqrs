namespace SaanSoft.Cqrs.Utilities;

public static class TypeExtensions
{
    public static string GetTypeFullName(this Type? type)
        => type == null
            ? string.Empty
            : type.FullName ?? type.Name;


    // TODO: this?
    public static string GetAssemblyName(this Type? type)
    {
        if (type == null) return string.Empty;

        var assemblyName = type.Assembly.GetName();
        return assemblyName.Name ?? assemblyName.FullName;
    }

    public static bool ImplementsGeneric(this Type type, Type openGeneric)
        => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
}

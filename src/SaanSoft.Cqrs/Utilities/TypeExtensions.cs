namespace SaanSoft.Cqrs.Utilities;

public static class TypeExtensions
{
    public static string GetTypeFullName(this Type? type)
        => type == null
            ? string.Empty
            : type.FullName ?? type.Name;


    public static string GetAssemblyName(this Type? type)
    {
        if (type == null) return string.Empty;

        var assemblyName = type.Assembly.GetName();
        return assemblyName.Name ?? assemblyName.FullName;
    }
}

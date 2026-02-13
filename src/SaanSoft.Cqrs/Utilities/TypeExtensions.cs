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

    // TODO: tests
    public static bool IsMessageWithoutResponse(this Type type)
        => type.IsCommand() || type.IsEvent();
    public static bool IsMessageWithResponse(this Type type)
        => type.IsCommandWithResponse() || type.IsQuery();
    public static bool IsCommand(this Type type)
        => type.Implements(typeof(ICommand)) && !type.IsCommandWithResponse();
    public static bool IsCommandWithResponse(this Type type)
        => type.ImplementsGeneric(typeof(ICommand<>));
    public static bool IsQuery(this Type type)
        => type.ImplementsGeneric(typeof(IQuery<>));
    public static bool IsEvent(this Type type)
        => type.Implements(typeof(IEvent));

    /// <summary>
    /// Check if 'type' implements the provided interface type.
    /// </summary>
    public static bool Implements(this Type type, Type interfaceType)
        => interfaceType.IsAssignableFrom(type);

    /// <summary>
    /// Check if 'type' implements the open generic interface 'openGeneric'.
    /// </summary>
    public static bool ImplementsGeneric(this Type type, Type openGeneric)
        => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);
}

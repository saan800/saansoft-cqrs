using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.DependencyInjection.ServiceProvider;

// TODO: add helper to add default services and config?

/// <summary>
/// Minimal service registry backed by IServiceProvider.
/// </summary>
public sealed class ServiceProviderRegistry(IServiceProvider serviceProvider) : IServiceRegistry
{
    public bool HasCommandHandler(Type commandType)
        => ResolveSingleHandler(typeof(ICommandHandler<>).MakeGenericType(commandType)) != null;

    public bool HasCommandResultHandler(Type commandType)
    {
        var i = commandType.GetInterfaces().FirstOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(ICommand<>)
            );
        if (i == null) return false;

        var tResult = i.GetGenericArguments()[0];
        var hType = typeof(ICommandHandler<,>).MakeGenericType(commandType, tResult);
        return ResolveSingleHandler(hType) != null;
    }

    public bool HasQueryHandler(Type queryType)
    {
        var i = queryType.GetInterfaces().FirstOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IQuery<>)
            );
        if (i == null) return false;

        var tResult = i.GetGenericArguments()[0];
        var hType = typeof(IQueryHandler<,>).MakeGenericType(queryType, tResult);
        return ResolveSingleHandler(hType) != null;
    }

    public bool HasEventHandlers(Type eventType)
        => ResolveMultipleHandlers(typeof(IEventHandler<>).MakeGenericType(eventType)).Any();

    public object? ResolveSingleHandler(Type handlerType)
    {
        var resolved = ResolveMultipleHandlers(handlerType);
        return resolved.Count() > 1
            ? throw new ApplicationException($"Multiple handlers found for {handlerType.FullName}")
            : resolved.SingleOrDefault();
    }

    public IEnumerable<object> ResolveMultipleHandlers(Type handlerType)
    {
        // TODO: see if can get actual IXxxHandler<> here, rather than IEnumerable<object>
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerType);
        if (serviceProvider.GetService(enumerableType) is not System.Collections.IEnumerable resolved) yield break;
        foreach (var item in resolved) yield return item!;
    }

    public T? ResolveService<T>() => serviceProvider.GetService<T>();

    public T ResolveRequiredService<T>() where T : notnull => serviceProvider.GetRequiredService<T>();
}

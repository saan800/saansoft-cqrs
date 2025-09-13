namespace SaanSoft.Cqrs.Bus.Utilities;

public static class MiddlewareExtensions
{
    public static Task RunPipeline(IEnumerable<Func<Func<Task>, Func<Task>>>? middlewares, Func<Task> terminal)
    {
        var next = terminal;
        if (middlewares != null)
        {
            foreach (var middleware in middlewares.Reverse())
                next = middleware(next);
        }
        return next();
    }

    /// <summary>
    /// Get the middlewares that match the give type of message
    /// </summary>
    public static IEnumerable<TMiddleware> GetValidMiddlewares<TMiddleware, TMiddlewareAll>(
        this IEnumerable<TMiddlewareAll>? originalMiddlewares
    )
    {
        var validMiddlewares = new List<TMiddleware>();
        foreach (var mw in originalMiddlewares ?? [])
        {
            if (mw is TMiddleware typed)
            {
                validMiddlewares.Add(typed);
            }
        }
        return validMiddlewares;
    }
}

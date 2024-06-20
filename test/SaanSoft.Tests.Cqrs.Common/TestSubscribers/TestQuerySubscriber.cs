using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestSubscribers;

public class TestQuerySubscriber(IServiceProvider serviceProvider) : IQuerySubscriber<Guid>
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default) where TQuery : IQuery<TQuery, TResponse>, IQuery<Guid>, IMessage<Guid> where TResponse : IQueryResponse
        => await GetHandler<TQuery, TResponse>().HandleAsync(query, cancellationToken);

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>() where TQuery : IQuery<TQuery, TResponse>, IQuery<Guid>, IMessage<Guid> where TResponse : IQueryResponse
    {
        var handlers = serviceProvider.GetServices<IQueryHandler<TQuery, TResponse>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                return handlers.Single();
            case 0:
                throw new InvalidOperationException($"No service for type '{typeof(IQueryHandler<TQuery, TResponse>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(x => x.GetType().FullName).ToList();
                    throw new InvalidOperationException($"Only one service for type '{typeof(IQueryHandler<TQuery, TResponse>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}

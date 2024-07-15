using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;
using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Models;
using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Queries;
using SaanSoft.Cqrs.GuidIds.Decorator.Store;
using SaanSoft.Cqrs.GuidIds.Messages;
using SaanSoft.Cqrs.Core.Handler;

namespace SaanSoft.Cqrs.Examples.Shared.Identity.Handlers;

public class UserQueryHandlers(IEventRepository eventRepository) :
    IQueryHandler<GetUserQuery, User?>,
    IQueryHandler<GetUserSummaryQuery, UserSummary?>,
    IQueryHandler<CheckUserUserNameExistsQuery, Guid?>
{
    public Task<User?> HandleAsync(GetUserQuery query, CancellationToken cancellationToken = default)
        => eventRepository.BuildEntity<User>(query.UserKey, cancellationToken);


    public Task<Guid?> HandleAsync(CheckUserUserNameExistsQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.UserName)) return Task.FromResult<Guid?>(null);

        var users = eventRepository.AsQueryable()
            .Where(evt => evt is UserCreatedEvent || evt is UserUpdatedEvent)
            .ToList()
            .Select(evt =>
            {
                return evt switch
                {
                    UserCreatedEvent created => new { created.Key, created.UserName},
                    UserUpdatedEvent updated => new { updated.Key, updated.UserName},
                    _ => null
                };
            })
            .Where(u => !string.IsNullOrWhiteSpace(u.UserName));

        var userKey = users
            .FirstOrDefault(u => string.Equals(u.UserName, query.UserName, StringComparison.OrdinalIgnoreCase))
            ?.Key;

        return Task.FromResult(userKey);
    }


    public async Task<UserSummary?> HandleAsync(GetUserSummaryQuery query, CancellationToken cancellationToken = default)
    {
        var user = await eventRepository.BuildEntity<User>(query.UserKey, cancellationToken);
        return user == null
            ? null
            : new UserSummary
                {
                    Key = user.Key,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
    }
}

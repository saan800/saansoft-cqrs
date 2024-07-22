using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;
using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;
using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Queries;
using SaanSoft.Cqrs.Handler;

namespace SaanSoft.Cqrs.Examples.Shared.Identity.Handlers;

public class UserCommandHandlers(IQueryBus queryBus, IEventBus eventBus) :
    ICommandHandler<CreateUserCommand>,
    ICommandHandler<UpdateUserCommand>,
    ICommandHandler<ForgetUserCommand>
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // validate supplied details
        if (string.IsNullOrWhiteSpace(command.UserName))
        {
            throw new ArgumentException($"{nameof(command.UserName)} must be provided with {nameof(CreateUserCommand)}");
        }
        if (string.IsNullOrWhiteSpace(command.FirstName))
        {
            throw new ArgumentException($"{nameof(command.FirstName)} must be provided with {nameof(CreateUserCommand)}");
        }
        if (string.IsNullOrWhiteSpace(command.LastName))
        {
            throw new ArgumentException($"{nameof(command.LastName)} must be provided with {nameof(CreateUserCommand)}");
        }

        // ensure not one with same username exists
        var existingUserKey = await queryBus.FetchAsync(new CheckUserUserNameExistsQuery(command.UserName), cancellationToken);
        if (existingUserKey != null)
        {
            throw new ArgumentException($"{nameof(command.UserName)} is already used by another user");
        }

        var userKey = Guid.NewGuid();
        await eventBus.QueueAsync(new UserCreatedEvent(userKey, command), cancellationToken);
    }

    public async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        // validate supplied details
        if (string.IsNullOrWhiteSpace(command.UserName))
        {
            throw new ArgumentException($"{nameof(command.UserName)} must be provided with {nameof(UpdateUserCommand)}");
        }
        if (string.IsNullOrWhiteSpace(command.FirstName))
        {
            throw new ArgumentException($"{nameof(command.FirstName)} must be provided with {nameof(UpdateUserCommand)}");
        }
        if (string.IsNullOrWhiteSpace(command.LastName))
        {
            throw new ArgumentException($"{nameof(command.LastName)} must be provided with {nameof(UpdateUserCommand)}");
        }

        // ensure not one with same username exists
        var existingUserKey = await queryBus.FetchAsync(new CheckUserUserNameExistsQuery(command.UserName), cancellationToken);
        if (existingUserKey != null && existingUserKey != command.UserKey)
        {
            throw new ArgumentException($"{nameof(command.UserName)} is already used by another user");
        }

        await eventBus.QueueAsync(new UserUpdatedEvent(command), cancellationToken);
    }

    public Task HandleAsync(ForgetUserCommand command, CancellationToken cancellationToken = default)
        => eventBus.QueueAsync(new UserForgottenEvent(command.UserKey, command), cancellationToken);
}

using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;

public class UserUpdatedEvent : Event
{
    public UserUpdatedEvent(Guid userKey, string? correlationId = null, string? authenticatedId = null)
        : base(userKey, correlationId, authenticatedId)
    {
    }

    public UserUpdatedEvent(UpdateUserCommand command) : base(command.UserKey, command)
    {
        UserName = command.UserName;
        FirstName = command.FirstName;
        LastName = command.LastName;
    }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Biography { get; set; }

    public User? ApplyEvent(User? entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity), $"User was null when applying {nameof(UserUpdatedEvent)}");

        entity.UserName = UserName;
        entity.FirstName = FirstName;
        entity.LastName = LastName;
        entity.Biography = Biography;
        entity.LastUpdatedOnUtc = MessageOnUtc;
        return entity;
    }
}

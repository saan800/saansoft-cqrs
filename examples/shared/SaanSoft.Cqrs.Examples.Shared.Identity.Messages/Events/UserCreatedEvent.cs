using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;

public class UserCreatedEvent : Event
{
    public UserCreatedEvent(Guid userKey, string? correlationId = null, string? authenticatedId = null)
        : base(userKey, correlationId, authenticatedId)
    {
    }

    public UserCreatedEvent(Guid userKey, CreateUserCommand command) : base(userKey, command)
    {
        UserName = command.UserName;
        FirstName = command.FirstName;
        LastName = command.LastName;
    }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public User? ApplyEvent(User? entity)
    {
        if (entity != null) throw new ArgumentOutOfRangeException(nameof(entity), $"User was not null when applying {nameof(UserCreatedEvent)}");

        return new User
        {
            Key = Key,
            UserName = UserName,
            FirstName = FirstName,
            LastName = LastName,
            CreatedOnUtc = MessageOnUtc,
            CreatedBy = TriggeredByUser ?? Key.ToString(),
            LastUpdatedOnUtc = MessageOnUtc
        };
    }
}

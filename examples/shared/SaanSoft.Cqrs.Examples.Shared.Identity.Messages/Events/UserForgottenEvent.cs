using SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;

public class UserForgottenEvent(ForgetUserCommand command) :
    Event(command.UserKey, command),
    IEntityEvent<User>
{
    public User? ApplyEvent(User? entity) => null;
}

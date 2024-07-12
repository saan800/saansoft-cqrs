using SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Models;

namespace SaanSoft.Cqrs.Examples.Shared.Todo.Entities;

public class TodoItem
{
    public Guid Key { get; set; }

    public string Title { get; set; }

    public int Order { get; set; }

    public TodoUser? AssignedTo { get; set; }

    public TodoStatus Status { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public TodoUser CreatedBy { get; set; }
}

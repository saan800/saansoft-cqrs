namespace SaanSoft.Cqrs.Examples.Shared.Todo.Entities;

public class TodoList
{
    public Guid Key { get; set; }

    public string Title { get; set; }

    public List<TodoItem> Items { get; set; } = [];

    public DateTime CreatedOnUtc { get; set; }

    public TodoUser CreatedBy { get; set; }
}

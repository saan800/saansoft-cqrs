namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Models;

public class TodoListSummary
{
    public Guid Key { get; set; }

    public string Title { get; set; }

    /// <summary>
    /// Number of items that have been completed
    /// </summary>
    public int CompleteItems { get; set; }

    /// <summary>
    /// Total number of items in the list
    /// </summary>
    public int TotalItems { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Name of the user who created the list
    /// </summary>
    public string CreatedBy { get; set; }
}

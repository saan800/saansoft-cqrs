namespace SaanSoft.Cqrs.Decorator.Store.Models;

public class MessageHandler
{
    public required string TypeFullName { get; set; }

    public required string Assembly { get; set; }

    public required DateTime HandledOnUtc { get; set; }

    public bool Succeeded { get; set; }

    public LogException? Exception { get; set; }

    public class LogException
    {
        public required string Message { get; set; }
        public required string TypeFullName { get; set; }
    }
}

using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeCommandBusDecorator(ILogger logger, ICommandBus next) :
    LoggerScopeCommandBusDecorator<Guid>(logger, next),
    ICommandBusDecorator;

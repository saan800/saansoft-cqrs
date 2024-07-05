using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeEventBusDecorator(ILogger logger, IEventBus next) :
    LoggerScopeEventBusDecorator<Guid>(logger, next),
    IEventBusDecorator;

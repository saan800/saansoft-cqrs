using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeQueryBusDecorator(ILogger logger, IQueryBus next) :
    LoggerScopeQueryBusDecorator<Guid>(logger, next),
    IQueryBusDecorator;

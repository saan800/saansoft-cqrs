using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeQueryBus(ILogger logger, IQueryBus next) :
    LoggerScopeQueryBus<Guid>(logger, next),
    IQueryBus;

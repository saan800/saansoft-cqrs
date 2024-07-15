using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeEventBus(ILogger logger, IEventBus next) :
    LoggerScopeEventBus<Guid>(logger, next),
    IEventBus;

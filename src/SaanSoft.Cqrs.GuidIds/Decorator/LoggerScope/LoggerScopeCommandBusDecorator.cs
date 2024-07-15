using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeCommandBus(ILogger logger, ICommandBus next) :
    LoggerScopeCommandBus<Guid>(logger, next),
    ICommandBus;

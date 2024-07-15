using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeCommandSubscriptionBus(ILogger logger, ICommandSubscriptionBus next) :
    LoggerScopeCommandSubscriptionBus<Guid>(logger, next),
    ICommandSubscriptionBus;

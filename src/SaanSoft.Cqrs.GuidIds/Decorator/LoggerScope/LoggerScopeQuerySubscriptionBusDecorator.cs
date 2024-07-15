using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeQuerySubscriptionBus(ILogger logger, IQuerySubscriptionBus next) :
    LoggerScopeQuerySubscriptionBus<Guid>(logger, next),
    IQuerySubscriptionBus;

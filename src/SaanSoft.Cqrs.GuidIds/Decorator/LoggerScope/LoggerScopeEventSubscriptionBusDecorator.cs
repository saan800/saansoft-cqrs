using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeEventSubscriptionBus(ILogger logger, IEventSubscriptionBus next) :
    LoggerScopeEventSubscriptionBus<Guid>(logger, next),
    IEventSubscriptionBus;

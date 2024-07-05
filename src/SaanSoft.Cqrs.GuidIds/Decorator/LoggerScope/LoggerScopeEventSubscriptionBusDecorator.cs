using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeEventSubscriptionBusDecorator(ILogger logger, IEventSubscriptionBus next) :
    LoggerScopeEventSubscriptionBusDecorator<Guid>(logger, next),
    IEventSubscriptionBusDecorator;

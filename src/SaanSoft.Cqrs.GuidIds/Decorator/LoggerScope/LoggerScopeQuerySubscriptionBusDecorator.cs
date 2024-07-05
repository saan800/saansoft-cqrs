using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeQuerySubscriptionBusDecorator(ILogger logger, IQuerySubscriptionBus next) :
    LoggerScopeQuerySubscriptionBusDecorator<Guid>(logger, next),
    IQuerySubscriptionBusDecorator;

using SaanSoft.Cqrs.Decorator.LoggerScope;

namespace SaanSoft.Cqrs.GuidIds.Decorator.LoggerScope;

public class LoggerScopeCommandSubscriptionBusDecorator(ILogger logger, ICommandSubscriptionBus next) :
    LoggerScopeCommandSubscriptionBusDecorator<Guid>(logger, next),
    ICommandSubscriptionBusDecorator;

# Logger Scope decorator

## Purpose

Add the `Microsoft.Extensions.Logging.ILogger.BeginScope` to the decorator stack to ensure your logs have
consistent structured logging attributes added.

## Dependencies

* `ILogger` setup in IoC

## Recommended Order

Should be added near the top of the decorator stack, but after (if using) theses
decorators to ensure that logging the most complete information possible:
* [Ensure Message Id decorators](../EnsureMessageId/README.md)
* [Ensure Correlation Id decorators](../../SaanSoft.Cqrs.Decorator.EnsureCorrelationId/README.md)



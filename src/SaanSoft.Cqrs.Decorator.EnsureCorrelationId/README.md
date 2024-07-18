# Ensure CorrelationId decorator

## Purpose

In a distributed system it can be a challenge to trace HTTP requests and messages
through multiple microservices.

A `CorrelationId` bundles each logical transaction as it moves through multiple processors.
With this system, your client's requests are collected under one value for easier tracking 
and troubleshooting.

This decorator populates the message's `CorrelationId` property automatically (if it hasn't 
already been set of course) when sending a message. 

Depending on the `ICorrelationIdProvider` supplied you can extract the value
from HTTP request headers or OpenTelemetry tracing for greater ease of tracking the
transaction.

## Dependencies

The decorator can optionally be provided a list of `ICorrelationIdProvider`.

Providers will be run in order to attempt to find the most appropriate value to 
set the message's `CorrelationId` to.

If none of the `ICorrelationIdProvider` return a value, the message's `CorrelationId`
will be defaulted to `Guid.NewGuid.ToString()`.

TODO: 
* Add `SaanSoft.CorrelationIdProviders` repository
  * Move interface over (and update these decorators to use new package)
  * add generic guid provider
  * add other correlation id providers in there
    * http header
    * OpenTelemetry (maybe?)
* SubscriptionBus read from message, then auto populate on publisher bus?

## Recommended Order

The CorrelationId decorators should be at the top (or very near to) of
the decorator stacks.

If your also using the [LoggerScope decorators](../SaanSoft.Cqrs.Decorator/LoggerScope/README.md), add
the CorrelationId decorators higher in the decorator stack.





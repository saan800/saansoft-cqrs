# Store Decorators

## :fire: :fire: WARNING :fire: :fire:

If you are storing messages, ensure you are taking appropriate security measures for 
sensitive data such as PPI, usernames, passwords, etc.

TODO:... Consider using the `Encryption` decorator to secure sensitive data.

## Purpose

Store messages, and optionally their publisher and handlers.

It is common in the CQRS patterns to use the event messages as the source of truth
for all data in the system.

SaanSoft.CQRS has decorators that you can apply to the message buses to store:
* Commands, Events, Queries
* Which class published the message
* What class(es) handled the message

You can use all of them or just the `StoreEventDecorator` for the most common configuration.

If you use the `Store[Command|Event|Query]PublisherDecorator` or the `Store[Command|Event|Query]HandlerDecorator`
you must also use the `Store[Command|Event|Query]Decorator` as the publisher and handler data
is stored in the message's `Metadata` property.

## Dependencies

The Store decorators require an implementation of the repository interfaces for your database (or other storage solution) of choice.

Check out [SaanSoft.Cqrs.Decorator.Store.MongoDB](../../SaanSoft.Cqrs.Decorator.Store.MongoDB)
and [SaanSoft.Cqrs.Decorator.Store.MongoDB.GuidIds](../../SaanSoft.Cqrs.Decorator.Store.MongoDB.GuidIds)
for an example.

## Recommended Order

The Store decorators should be applied near the bottom of the decorator stack.

You only want to store data if all the other decorator logic 
has been applied and the message is ready to be run.

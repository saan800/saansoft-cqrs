# Store Decorators

## :fire: :fire: WARNING :fire: :fire:

If you are storing messages, ensure you are taking appropriate security measures for 
sensitive data such as PPI, usernames, passwords, etc.

If your database technology offers an encrypted data option, that is a good solution.

TODO:... Otherwise consider using the `Encryption` decorator to secure sensitive data. 

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

Check [SaanSoft.Cqrs.Decorator.Store.MongoDB](../../SaanSoft.Cqrs.Decorator.Store.MongoDB)
for an example.

### Contributing a new implementation of repository interfaces

If you are using a different database/store than the ones already provided, please add the implementation for the repository interfaces so it's available to others.

All repository implementations should be tested. The 
[SaanSoft.Tests.Cqrs.Decorator.Store.BaseRepository](../../../test/SaanSoft.Tests.Cqrs.Decorator.Store.BaseRepository)
has the minimum base tests that all repository implementations should handle and pass. 

If there are extra requirements for a repository implementation (eg database indexing, ServiceCollection extension methods),
then add those to the repository specific test project.

Check [SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB](../../../test/SaanSoft.Tests.Cqrs.Decorator.Store.MongoDB)
for an example.

## Recommended Order

The Store decorators should be applied near the bottom of the decorator stack.

You only want to store data if all the other decorator logic 
has been applied and the message is ready to be run.

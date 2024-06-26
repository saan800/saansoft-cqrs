# SaanSoft.Cqrs.Decorator

This project is used for adding decorators for the message buses.

## Contributing

Each decorator (or set of decorators - i.e. possibly one each for command, event, query, bus and subscription bus) should be in its own folder and namespace.

Each decorator should be minimal and targeted to specific functionality.

In `SaanSoft.Cqrs.Decorator` new decorators shouldn't add non-generic packages to the project.
* Add general implementations and interfaces in `SaanSoft.Cqrs.Decorator`, then add another project for implementation specific framework. e.g:
  * The `Store` decorators classes and repository interfaces are in `SaanSoft.Cqrs.Decorator` because they don't require any extra package imports.
  * The database specific implementations are in their own project (e.g. `SaanSoft.Cqrs.Decorator.Store.MongoDB`)
* This keeps the footprint of `SaanSoft.Cqrs` to a minimum and allows users the flexibility to use the framework that they prefer.

## Technical

Each decorator should inherit from one of:
* ICommandBusDecorator
* ICommandSubscriptionBusDecorator
* IEventBusDecorator
* IEventSubscriptionBusDecorator
* IQueryBusDecorator
* IQuerySubscriptionBusDecorator

If you are using `Base...Decorator` classes to reduce code duplicate, it should inherit form one of:
* IMessageBusDecorator
* IMessageSubscriptionBus

Each decorator and framework specific implementations should be tested.

## Documentation

When adding a decorator, please ensure appropriate documentation is added to the [docs](../../docs) folder.

Include information such as:
* What is the purpose of your decorator
* Does it require other packages to work (e.g. the `Stores` decorator requires a database implementation of the repositories)
* Is there a recommended order of adding the decorator to the message or subscription bus. e.g:
  * The `Store` decorator should be added near the bottom of the decorator stack near the actual implementation of the message or subscription bus.
    * But if using the `RetryPolicy` decorator (TODO...) it should be above that in the decorator stack.
  * The `LoggerScope` decorator should be near the top of the decorator stack, so any logs have as much information as possible.

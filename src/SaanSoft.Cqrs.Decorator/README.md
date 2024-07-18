# SaanSoft.Cqrs.Decorator

This project is used for adding decorators for the message buses.

For more information on each decorator please check the `README.md` in each deciratir folder.

## Contributing

Each decorator (or set of decorators - i.e. possibly one each for command, event, query bus and/or subscription bus) should be in its own folder and namespace.

Each decorator should be minimal and targeted to specific functionality.

In `SaanSoft.Cqrs.Decorator` new decorators shouldn't add non-generic packages to the project.
* Add general implementations and interfaces in `SaanSoft.Cqrs.Decorator`, then add another project for any implementation specific frameworks. e.g:
  * The `Store` decorator classes and repository interfaces are in `SaanSoft.Cqrs.Decorator` because they don't require any extra package imports.
  * The database specific implementations are in their own project (e.g. `SaanSoft.Cqrs.Decorator.Store.MongoDB`)
* This keeps the footprint of `SaanSoft.Cqrs` to a minimum and allows users the flexibility to use the framework that they prefer.

### Technical

Each decorator must inherit from one of:
* ICommandBus
* ICommandSubscriptionBus
* IEventBus
* IEventSubscriptionBus
* IQueryBus
* IQuerySubscriptionBus

If you are using `Base...Decorator` classes to reduce code duplication, it must inherit from:
* IDecorator

Each decorator and framework specific implementations should be tested.

TODO - Each new decorator project should be added to test decorator all to ensure its valid

### Documentation

When adding a decorator, please ensure an appropriate `README.md` file is added to the decorator folder.

Use [TEMPLATE.md](https://github.com/saan800/saansoft-cqrs/blob/main/src/SaanSoft.Cqrs.Decorator/TEMPLATE.md) as a starting point.

Include information such as:
* What is the purpose of your decorator
* Does it require other packages or decorators to work (e.g. the `Stores` decorator requires implementation of the repository interfaces)
* Is there a recommended order of adding the decorator to the message or subscription bus. e.g:
  * The `Store` decorator should be added near the bottom of the decorator stack near the actual implementation of the message or subscription bus.
    * But if using the `RetryPolicy` decorator (TODO...) it should be above that in the decorator stack.
  * The `LoggerScope` decorator should be near the top of the decorator stack but after the `EnsureMessageId` and `EnsureCorrelationId` decorators, so any logs have as much information as possible.

TODO..
Add a link to the decorator readme from the documentation list of decorators in the recommended order

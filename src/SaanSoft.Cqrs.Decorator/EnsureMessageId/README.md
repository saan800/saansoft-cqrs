# Ensure Message Id Decorators

## Purpose

Ensure your message have the Id field populated.

When logging or storing the messages, it can be useful to have an Id for each individual
message. 

Also the message's Id can be applied to other triggered messages metadata when using
the message constructor with `triggeredByMessage` argument (or apply it yourself of course).

This can help with traceability and debugging your app as you can see the
exact change of messages that occurred.

## Dependencies

* `IIdGenerator<TMessageId>`

## Recommended Order

The Ensure Message Id decorators should be at the top (or very near to) of
the `IMessageBus` decorator stack.

This ensures that things like logging will have as much useful data as it
can get.

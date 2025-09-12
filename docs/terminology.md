# Terminology

## Aggregate Root

Represents all event message that relate to a specific "entity" - Order, BlogPost, User

## Command

Commands are used for actions in the system that should happen - CreateOrder, ProcessPayment, DeleteBlogPost, UploadFile

Handling a command should include validation and other business logic to ensure its valid to continue.

Command handling should not alter any state in the system, however things that should only ever happen once (ie if 
messages are being replayed, then don't re-run the command logic). eg

* Processing payments
* Send emails

A command handler will often raise one or more associated events if the command validation and
business logic is successful.

## Event

Events are a record of what has happened in the system - OrderCreated, PaymentProcessed, BlogDetailsUpdated, FileUploaded

Event handling should update the system (ie database or other storage writes), but not perform any (or at least very minimal) business logic.

Events should be safe to replay, and the aggregate root of events being replayed should result in the same entity state.

## Query

Queries should only ever be "fetch" and never alter the state of the system.

While replaying queries should be safe, it is unlikely to happen except when event handlers publish queries to 
fetch additional data.


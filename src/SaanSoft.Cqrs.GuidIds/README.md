# SaanSoft.Cqrs.GuidIds

SaanSoft.Cqrs tries to be as un-opinionated as possible. You get to choose the functionality you want
and how it applies to your project.

You can choose which type the Ids you are using in your project, but it can be 
messy to deal with `TMessageId` generics everywhere.

`SaanSoft.Cqrs.GuidIds` extends `SaanSoft.Cqrs` and `SaanSoft.Cqrs.Decorator` 
projects and classes to add `TMessageId=Guid` implementations and base classes.

If using `TMessageId=Guid`, its preferable to use the interfaces and base classes from this project
for neater coding.

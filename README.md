# SaanSoft.Cqrs

Lightweight CQRS and Event Sourcing implementation for C#

## Status

[![build-and-test](https://github.com/saan800/saansoft-cqrs/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/saan800/saansoft-cqrs/actions/workflows/build-and-test.yml) 
[![codecov](https://codecov.io/gh/saan800/saansoft-cqrs/graph/badge.svg?token=FIHYI10VIW)](https://codecov.io/gh/saan800/saansoft-cqrs)
[![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/saan800/saansoft-cqrs/badge)](https://securityscorecards.dev/viewer/?uri=github.com/saan800/saansoft-cqrs)


## Documentation

Todo...
* event source vs event stream vs event driven
* cqrs
* glossary
* index with links to implemented decorators (instead of in this file)

## TODO

LocalBus for publishers
- tryExecute / queue / fetch

all messages - attribute to have 3rd party queue group name (for aws/azure/etc)

## Features

* Replay events
* replay events for new handlers

### Decorators

* [Store](./src/SaanSoft.Cqrs.Decorator/Store/README.md) - Store messages, and optionally their publisher and handlers
* [ILogger Scope](./src/SaanSoft.Cqrs.Decorator/LoggerScope/README.md)
  * Ensure useful structured logging by adding `ILogger.BeginScope` with message metadata
* [Ensure CorrelationId](./src/SaanSoft.Cqrs.Decorator.EnsureCorrelationId/README.md) <!--  can we do similar for auth id? -->
* retry with polly

### Message Repository

* MongoDB
* AWS DynamoDB
* Azure Cosmos DB

### Message Buses

* :eyes: [Local](./src/SaanSoft.Cqrs/Bus)
* AWS SNS/SQS
* Azure Topics and Queues (maybe, plus example app if doing)

### DI frameworks TODO:

* :eyes: c# IServiceCollection
* SimpleInjection

## Examples TODO:

### Shared api and messaging functionality

:eyes:

Users

* User - {Key, Name}
* CRUD

TodoList

* TodoItem - {Key, Title, Order, AssignedTo?=User, Status=New|InProgress|Done}
  * Add/edit/delete
  * AssignToUser
  * UpdateStatus
  * UpdateOrder
* TodoList - {Key, Title}
  * Add/edit/delete
  * Get all lists
  * Get all lists with items assigned to user
  * Get by list Key
* Maybe in future
  * Add an image to the TodoItem, see how its dealt with in upload and message processing

### Basic Local/InMemory Api

* local app messaging
* no decorators
* no message store
* MongoDB for read model for queries 

### Api endpoint tests

* Run Api in Github Actions CI
  * Will eventually want to run a test matrix of Api tests
* Run same tests against each example Api to ensure functionality is the same from a users perspective
  * Specflow + Playwright
* Enrich each test matrix to ensure decorators/db/etc are working

### MongoDB Store Api

* local app messaging
* all the decorators
* mongodb repositories 
* use messages to build read model in queries
* diagnostic/info flowchart of messages
  * filter by
    * to from date
    * message full type name
    * correlation id
    * assembly for publisher/handler??
    * has error 
  * return [mermaid](https://mermaid.js.org/syntax/flowchart.html) text from api
  * maybe [svelvet](https://svelvet.mintlify.app/components/node) for ui
* Use [docker compose](https://www.mongodb.com/resources/products/compatibilities/docker) local dev and GitHub CI infrastructure

### Aws Lambda Api and SNS/SQS handlers

* Aws SNS/SQS messaging
* all the decorators except stores
* queries read model from ??
* Use [LocalStack](https://www.localstack.cloud/) for local dev and GitHub CI infrastructure
  * [Docker compose](https://docs.localstack.cloud/getting-started/installation/#docker-compose)

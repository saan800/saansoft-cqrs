# SaanSoft.Cqrs

CQRS and Event Sourcing implementation for C#

## Status

[![build-and-test](https://github.com/saan800/saansoft-cqrs/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/saan800/saansoft-cqrs/actions/workflows/build-and-test.yml) 
[![codecov](https://codecov.io/gh/saan800/saansoft-cqrs/graph/badge.svg?token=FIHYI10VIW)](https://codecov.io/gh/saan800/saansoft-cqrs)
[![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/saan800/saansoft-cqrs/badge)](https://securityscorecards.dev/viewer/?uri=github.com/saan800/saansoft-cqrs)
[![OpenSSF Best Practices](https://www.bestpractices.dev/projects/8734/badge)](https://www.bestpractices.dev/projects/8734)


## Documentation

Todo...

## TODO

Decorator - generate Id

IInMemory***Bus for publishers
- tryExecute / queue / fetch

all messages - attribute to have queue group name (for aws/azure/etc)

event handlers - have attribute on handler that indicates running order priority

## Feature ideas

### Other

* Replay events
* new scheduled command message type
  * runs in memory only

### Decorators

* Generate Id - publishers
* :eyes: Store - Store messages, publishers and handlers
* ILogger Scope
* CorrelationId provider
  * Guid
  * http header
  * OpenTelemetry?
  * SubscriptionBus read from message, then auto populate on publisher bus?
  * can we do same for auth id?
* Encryption / Decryption
* retry with polly
* Metrics
  * Track: number of messages, time to process, success/failure
  * on both message bus and subscription bus
  * OpenTelemetry
  * Azure AppInsight (think this might be able to work from open telemetry)

### Message Stores

* :eyes: MongoDB
* AWS DynamoDB (maybe, plus example app if doing)
* Azure Cosmos DB (maybe, plus example app if doing)

### Message Buses

* :white_check_mark: In Memory
* AWS SNS/SQS
* Azure Topics and Queues (maybe, plus example app if doing)

### DI frameworks

* :eyes: c# IServiceCollection
* SimpleInjection

## Examples

### Shared api and messaging functionality

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

### Basic InMemory Api

* in memory messaging
* no decorators
* no store
* MongoDB for read model for queries 

### Api endpoint tests

* Run Api in Github Actions CI
  * Will eventually want to run a test matrix of Api tests
* Run same tests against each example Api to ensure functionality is the same from a users perspective
  * Specflow + Playwright
* Enrich each test matrix to ensure decorators/db/etc are working

### MongoDB Store Api

* in memory messaging
* all the decorators
* mongodb repositories 
* use event messages to build read model in queries
* Use [docker compose](https://www.mongodb.com/resources/products/compatibilities/docker) local dev and GitHub CI infrastructure

### Aws Lambda Api and SNS/SQS handlers

* Aws SNS/SQS messaging
* all the decorators except stores
* queries read model from ??
* Use [LocalStack](https://www.localstack.cloud/) for local dev and GitHub CI infrastructure
  * [Docker compose](https://docs.localstack.cloud/getting-started/installation/#docker-compose)

# saansoft-cqrs

CQRS and Event Sourcing implementation for C#

## Status

[![build-and-test](https://github.com/saan800/saansoft-cqrs/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/saan800/saansoft-cqrs/actions/workflows/build-and-test.yml) 
[![codecov](https://codecov.io/gh/saan800/saansoft-cqrs/graph/badge.svg?token=FIHYI10VIW)](https://codecov.io/gh/saan800/saansoft-cqrs)
[![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/saan800/saansoft-cqrs/badge)](https://securityscorecards.dev/viewer/?uri=github.com/saan800/saansoft-cqrs)
[![OpenSSF Best Practices](https://www.bestpractices.dev/projects/8734/badge)](https://www.bestpractices.dev/projects/8734)


## Documentation

Todo...

## TODO

IInMemery***Bus for publishers
- tryExecute / queue / fetch

split guid to own project
add IIdGenerator interface

decorator project readme
- no new packages
- add doc for each decirator

use IMessageBus naming for publishers

stores => repository 

no base interface for repos, use types for messages

split mongo store into 3

event handlers - have attribute on handler that indicates running order priority

signal r example
AWS Gateway example for web sockets

## Feature ideas

### Other

* Event sourcing
* Replay

### Decorators

* :eyes: Store - Store messages, publishers and subscribers
* ILogger Scope
* CorrelationId
* Encryption / Decryption
* retry with polly
* Metrics
  * Track: number of messages, time to process, success/failure
  * on both bus and subscribers
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

## Examples

### Shared api and messaging functionality

Users

* User - {Key, Name, Username}
* CRUD

TodoList

* TodoItem - {Id, Title, Order, AssignedTo?: User, DueDate?, Status=New|InProgress|Done}
  * Add/edit/delete
  * AssignToUser
  * UpdateStatus
* TodoList - {Key, Title}
  * Add/edit/delete
  * Get all lists
  * Get all lists assigned to user
  * Get by list Key
* Maybe in future
  * Add an image to the TodoItem, see how its dealt with in upload and message processing

### Basic InMemory Api

* in memory messaging
* no decorators
* no store

### Api endpoint tests

* Run Api in Github Actions CI
  * Will eventually want to run a test matrix of Api tests
* Run same tests against each example Api to ensure functionality is the same from a users perspective
  * Specflow + Playwright
* Enrich each test matrix to ensure decorators/db/etc are working

### MongoDB Store Api

* in memory messaging
* all the decorators
* mongodb store
* Use [docker compose](https://www.mongodb.com/resources/products/compatibilities/docker) local dev and GitHub CI infrastructure

### Aws Lambda Api and SNS/SQS handlers

* Aws SNS/SQS messaging
* all the decorators except stores
* Use [LocalStack](https://www.localstack.cloud/) for local dev and GitHub CI infrastructure
  * [Docker compose](https://docs.localstack.cloud/getting-started/installation/#docker-compose)

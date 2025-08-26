# Akka.NET SAGA Implementation

This project now includes a comprehensive SAGA (Saga Pattern) implementation using Akka.NET actors for managing distributed transactions in the microservice architecture.

## Overview

The SAGA pattern is used to manage data consistency across multiple services in distributed transactions. This implementation provides:

- **Choreography-based SAGA**: Each service manages its own transactions and publishes events
- **Compensation logic**: Automatic rollback of completed steps when failures occur
- **Actor-based coordination**: Using Akka.NET actors for reliable message processing
- **Event sourcing ready**: Foundation for event sourcing implementation

## Architecture

### Core Components

1. **SagaCoordinatorActor**: Manages the lifecycle of individual SAGAs
2. **EisenSagaActor**: Handles business logic for Eisen (project/requirement) creation
3. **SagaManagerActor**: Manages multiple SAGA instances
4. **SagaService**: Service layer for interacting with SAGA actors from controllers

### Message Types

- **StartSaga**: Initiates a new SAGA
- **CompleteSagaStep**: Marks a step as completed
- **CompensateSagaStep**: Triggers compensation (rollback) for a step
- **SagaCompleted/SagaFailed**: Final SAGA state notifications

### SAGA Flow Example (Eisen Creation)

1. **Validation Step**: Validates the Eisen data
2. **Persistence Step**: Saves the Eisen to the database (simulated)
3. **Notification Step**: Sends external notifications (simulated)

If any step fails, compensation logic automatically rolls back completed steps.

## API Endpoints

### Create Eisen with SAGA
```
POST /api/eisen
Content-Type: application/json

{
  "ID": "3.0.0",
  "Title": "Quantum Computer",
  "ModelImage": "https://example.com/quantum.jpg",
  "Description": "Build a universal quantum computer",
  "Phase": "Research ontwerp"
}
```

Response:
```json
{
  "sagaId": "7c831f26-39c2-4572-9d63-92b13c388487",
  "message": "Eisen creation SAGA started successfully",
  "eisenData": { ... }
}
```

### Check SAGA Status
```
GET /api/eisen/saga/{sagaId}/status
```

Response:
```json
{
  "sagaId": "7c831f26-39c2-4572-9d63-92b13c388487",
  "correlationId": "7ce764bd-17d1-4b07-8884-b8e8d36d5d20",
  "status": 2,
  "data": { ... }
}
```

Status values:
- 0: NotStarted
- 1: InProgress
- 2: Completed
- 3: Failed
- 4: Compensating
- 5: Compensated

### Get Active SAGAs Count
```
GET /api/eisen/sagas/count
```

Response:
```json
{
  "activeSagasCount": 1
}
```

## Configuration

The Akka.NET ActorSystem is configured in `Startup.cs`:

```csharp
var akkaConfig = ConfigurationFactory.ParseString(@"
    akka {
        actor {
            provider = ""Akka.Actor.LocalActorRefProvider""
        }
        loglevel = INFO
    }
");

var actorSystem = ActorSystem.Create("SagaSystem", akkaConfig);
```

## Extending the SAGA

To add new SAGA types:

1. Create message types in `Sagas/Messages/`
2. Create actor class in `Sagas/Actors/`
3. Add handling in `SagaManagerActor`
4. Create service methods in `SagaService`
5. Add controller endpoints

## Benefits

- **Reliability**: Akka.NET provides supervision and restart strategies
- **Scalability**: Actors can be distributed across cluster nodes
- **Maintainability**: Clear separation of concerns between steps
- **Observability**: Built-in logging and message tracing
- **Fault tolerance**: Automatic compensation on failures

## Future Enhancements

- Event Store integration for event sourcing
- Cluster deployment for high availability
- SAGA persistence for durable state management
- Monitoring and metrics collection
- Web UI for SAGA visualization
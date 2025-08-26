using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using NCoreWebApp.Sagas.Messages;

namespace NCoreWebApp.Sagas.Actors
{
    /// <summary>
    /// State data for SAGA coordinator
    /// </summary>
    public record SagaState(
        Guid SagaId,
        Guid CorrelationId,
        SagaStatus Status,
        Dictionary<string, object> StepResults,
        List<string> CompletedSteps,
        List<string> CompensationSteps,
        object? InitialData = null,
        Exception? LastError = null
    );

    /// <summary>
    /// SAGA Coordinator Actor that manages the execution of a distributed transaction
    /// </summary>
    public class SagaCoordinatorActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private SagaState _state;

        public SagaCoordinatorActor(Guid sagaId, Guid correlationId)
        {
            _state = new SagaState(
                sagaId,
                correlationId,
                SagaStatus.NotStarted,
                new Dictionary<string, object>(),
                new List<string>(),
                new List<string>()
            );

            Become(WaitingForStart);
        }

        private void WaitingForStart()
        {
            Receive<StartSaga>(msg =>
            {
                _log.Info($"Starting SAGA {msg.SagaId} with correlation {msg.CorrelationId}");
                _state = _state with 
                { 
                    Status = SagaStatus.InProgress, 
                    InitialData = msg.InitialData 
                };
                Become(InProgress);
                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, SagaStatus.InProgress));
            });

            Receive<GetSagaStatus>(msg =>
            {
                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, _state.Status, _state.InitialData));
            });
        }

        private void InProgress()
        {
            Receive<CompleteSagaStep>(msg =>
            {
                _log.Info($"Completing step {msg.StepName} for SAGA {msg.SagaId}");
                
                var newCompletedSteps = new List<string>(_state.CompletedSteps) { msg.StepName };
                var newStepResults = new Dictionary<string, object>(_state.StepResults);
                
                if (msg.Result != null)
                {
                    newStepResults[msg.StepName] = msg.Result;
                }

                _state = _state with 
                { 
                    CompletedSteps = newCompletedSteps,
                    StepResults = newStepResults
                };

                // Check if all steps are completed (this is a simple implementation)
                // In a real scenario, you would define the SAGA workflow steps
                if (IsAllStepsCompleted())
                {
                    CompleteSaga();
                }

                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, _state.Status));
            });

            Receive<CompensateSagaStep>(msg =>
            {
                _log.Warning($"Compensating step {msg.StepName} for SAGA {msg.SagaId}");
                
                var newCompensationSteps = new List<string>(_state.CompensationSteps) { msg.StepName };
                _state = _state with 
                { 
                    Status = SagaStatus.Compensating,
                    CompensationSteps = newCompensationSteps
                };

                // Check if all compensations are done
                if (IsAllCompensationsCompleted())
                {
                    FailSaga("SAGA compensated due to failures");
                }

                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, _state.Status));
            });

            Receive<GetSagaStatus>(msg =>
            {
                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, _state.Status, _state.StepResults));
            });

            ReceiveAny(msg =>
            {
                _log.Warning($"Received unexpected message {msg.GetType().Name} in InProgress state");
            });
        }

        private void Completed()
        {
            Receive<GetSagaStatus>(msg =>
            {
                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, _state.Status, _state.StepResults));
            });

            ReceiveAny(msg =>
            {
                _log.Warning($"SAGA {_state.SagaId} is completed but received message {msg.GetType().Name}");
            });
        }

        private void Failed()
        {
            Receive<GetSagaStatus>(msg =>
            {
                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, _state.Status, _state.LastError?.Message));
            });

            ReceiveAny(msg =>
            {
                _log.Warning($"SAGA {_state.SagaId} has failed but received message {msg.GetType().Name}");
            });
        }

        private bool IsAllStepsCompleted()
        {
            // Simple check - in a real implementation, you'd define the required steps
            return _state.CompletedSteps.Count >= 3; // Assuming 3 steps: Validate, Persist, Notify
        }

        private bool IsAllCompensationsCompleted()
        {
            // Simple check for compensations
            return _state.CompensationSteps.Count >= _state.CompletedSteps.Count;
        }

        private void CompleteSaga()
        {
            _log.Info($"SAGA {_state.SagaId} completed successfully");
            _state = _state with { Status = SagaStatus.Completed };
            
            // Notify interested parties
            Context.ActorSelection("/user/saga-manager").Tell(
                new SagaCompleted(_state.SagaId, _state.CorrelationId, _state.StepResults));
            
            Become(Completed);
        }

        private void FailSaga(string reason)
        {
            _log.Error($"SAGA {_state.SagaId} failed: {reason}");
            _state = _state with 
            { 
                Status = SagaStatus.Failed,
                LastError = new Exception(reason)
            };
            
            // Notify interested parties
            Context.ActorSelection("/user/saga-manager").Tell(
                new SagaFailed(_state.SagaId, _state.CorrelationId, reason));
            
            Become(Failed);
        }

        public static Props Props(Guid sagaId, Guid correlationId) =>
            Akka.Actor.Props.Create(() => new SagaCoordinatorActor(sagaId, correlationId));
    }
}
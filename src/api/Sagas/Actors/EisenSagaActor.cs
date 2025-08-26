using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using NCoreWebApp.Dtos;
using NCoreWebApp.Sagas.Messages;

namespace NCoreWebApp.Sagas.Actors
{
    /// <summary>
    /// Actor responsible for managing Eisen creation SAGA
    /// </summary>
    public class EisenSagaActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private IActorRef? _coordinatorRef;

        public EisenSagaActor()
        {
            Receive<CreateEisenCommand>(HandleCreateEisen);
            Receive<ValidateEisenCommand>(HandleValidateEisen);
            Receive<PersistEisenCommand>(HandlePersistEisen);
            Receive<NotifyEisenCreatedCommand>(HandleNotifyEisenCreated);
            Receive<RemoveEisenCommand>(HandleRemoveEisen);
            Receive<RollbackEisenNotificationCommand>(HandleRollbackNotification);
        }

        private void HandleCreateEisen(CreateEisenCommand cmd)
        {
            _log.Info($"Starting Eisen creation SAGA {cmd.SagaId} for Eisen: {cmd.EisenData.Title}");

            // Create or get reference to coordinator
            _coordinatorRef = Context.ActorOf(
                SagaCoordinatorActor.Props(cmd.SagaId, cmd.CorrelationId),
                $"saga-coordinator-{cmd.SagaId}");

            // Start the SAGA
            _coordinatorRef.Tell(new StartSaga(cmd.SagaId, cmd.CorrelationId, cmd.EisenData));

            // Begin with validation step
            Self.Tell(new ValidateEisenCommand(cmd.SagaId, cmd.CorrelationId, cmd.EisenData));
        }

        private void HandleValidateEisen(ValidateEisenCommand cmd)
        {
            _log.Info($"Validating Eisen for SAGA {cmd.SagaId}");

            try
            {
                // Simulate validation logic
                var isValid = ValidateEisenData(cmd.EisenData);
                var validationErrors = isValid ? null : "Title and Description are required";

                if (isValid)
                {
                    // Tell coordinator this step is complete
                    _coordinatorRef?.Tell(new CompleteSagaStep(cmd.SagaId, cmd.CorrelationId, "Validation", cmd.EisenData));
                    
                    // Proceed to next step
                    Self.Tell(new PersistEisenCommand(cmd.SagaId, cmd.CorrelationId, cmd.EisenData));
                    
                    // Notify completion
                    Sender.Tell(new EisenValidated(cmd.SagaId, cmd.CorrelationId, cmd.EisenData, true));
                }
                else
                {
                    _log.Warning($"Eisen validation failed for SAGA {cmd.SagaId}: {validationErrors}");
                    _coordinatorRef?.Tell(new CompensateSagaStep(cmd.SagaId, cmd.CorrelationId, "Validation", validationErrors));
                    Sender.Tell(new EisenValidated(cmd.SagaId, cmd.CorrelationId, cmd.EisenData, false, validationErrors));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error validating Eisen for SAGA {cmd.SagaId}");
                _coordinatorRef?.Tell(new CompensateSagaStep(cmd.SagaId, cmd.CorrelationId, "Validation", ex.Message));
                Sender.Tell(new EisenValidated(cmd.SagaId, cmd.CorrelationId, cmd.EisenData, false, ex.Message));
            }
        }

        private void HandlePersistEisen(PersistEisenCommand cmd)
        {
            _log.Info($"Persisting Eisen for SAGA {cmd.SagaId}");

            try
            {
                // Simulate persistence logic
                var persistenceId = PersistEisenData(cmd.EisenData);

                // Tell coordinator this step is complete
                _coordinatorRef?.Tell(new CompleteSagaStep(cmd.SagaId, cmd.CorrelationId, "Persistence", persistenceId));
                
                // Proceed to next step
                Self.Tell(new NotifyEisenCreatedCommand(cmd.SagaId, cmd.CorrelationId, cmd.EisenData));
                
                // Notify completion
                Sender.Tell(new EisenPersisted(cmd.SagaId, cmd.CorrelationId, cmd.EisenData, persistenceId));
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error persisting Eisen for SAGA {cmd.SagaId}");
                _coordinatorRef?.Tell(new CompensateSagaStep(cmd.SagaId, cmd.CorrelationId, "Persistence", ex.Message));
                
                // Trigger compensation for validation step
                Self.Tell(new CompensateSagaStep(cmd.SagaId, cmd.CorrelationId, "Validation"));
            }
        }

        private void HandleNotifyEisenCreated(NotifyEisenCreatedCommand cmd)
        {
            _log.Info($"Sending notifications for Eisen creation SAGA {cmd.SagaId}");

            try
            {
                // Simulate external notification logic
                var notificationSuccess = SendEisenCreatedNotification(cmd.EisenData);

                if (notificationSuccess)
                {
                    // Tell coordinator this step is complete
                    _coordinatorRef?.Tell(new CompleteSagaStep(cmd.SagaId, cmd.CorrelationId, "Notification", "Notification sent"));
                    
                    // Notify completion
                    Sender.Tell(new EisenNotificationSent(cmd.SagaId, cmd.CorrelationId, cmd.EisenData, true));
                }
                else
                {
                    _log.Warning($"Failed to send notification for SAGA {cmd.SagaId}");
                    _coordinatorRef?.Tell(new CompensateSagaStep(cmd.SagaId, cmd.CorrelationId, "Notification", "Notification failed"));
                    
                    // Trigger compensations for previous steps
                    Self.Tell(new RemoveEisenCommand(cmd.SagaId, cmd.CorrelationId, cmd.EisenData.ID));
                    
                    Sender.Tell(new EisenNotificationSent(cmd.SagaId, cmd.CorrelationId, cmd.EisenData, false));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error sending notification for SAGA {cmd.SagaId}");
                _coordinatorRef?.Tell(new CompensateSagaStep(cmd.SagaId, cmd.CorrelationId, "Notification", ex.Message));
                
                // Trigger compensations for previous steps
                Self.Tell(new RemoveEisenCommand(cmd.SagaId, cmd.CorrelationId, cmd.EisenData.ID));
            }
        }

        private void HandleRemoveEisen(RemoveEisenCommand cmd)
        {
            _log.Info($"Removing Eisen {cmd.EisenId} as compensation for SAGA {cmd.SagaId}");

            try
            {
                // Simulate removal logic
                RemovePersistedEisen(cmd.EisenId);
                _coordinatorRef?.Tell(new CompleteSagaStep(cmd.SagaId, cmd.CorrelationId, "RemovalCompensation", "Eisen removed"));
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error removing Eisen {cmd.EisenId} for SAGA {cmd.SagaId}");
            }
        }

        private void HandleRollbackNotification(RollbackEisenNotificationCommand cmd)
        {
            _log.Info($"Rolling back notification for Eisen {cmd.EisenId} as compensation for SAGA {cmd.SagaId}");

            try
            {
                // Simulate notification rollback logic
                RollbackNotification(cmd.EisenId);
                _coordinatorRef?.Tell(new CompleteSagaStep(cmd.SagaId, cmd.CorrelationId, "NotificationRollback", "Notification rolled back"));
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error rolling back notification for Eisen {cmd.EisenId} for SAGA {cmd.SagaId}");
            }
        }

        #region Simulation Methods

        private bool ValidateEisenData(EisDto eisen)
        {
            // Simple validation logic
            return !string.IsNullOrWhiteSpace(eisen.Title) && 
                   !string.IsNullOrWhiteSpace(eisen.Description);
        }

        private string PersistEisenData(EisDto eisen)
        {
            // Simulate database persistence
            var persistenceId = $"eisen-{Guid.NewGuid()}";
            _log.Info($"Persisted Eisen '{eisen.Title}' with ID: {persistenceId}");
            return persistenceId;
        }

        private bool SendEisenCreatedNotification(EisDto eisen)
        {
            // Simulate external notification (e.g., email, webhook, etc.)
            _log.Info($"Sending notification for Eisen creation: {eisen.Title}");
            return true; // Assume success for demo
        }

        private void RemovePersistedEisen(string eisenId)
        {
            // Simulate removal from database
            _log.Info($"Removed Eisen with ID: {eisenId}");
        }

        private void RollbackNotification(string eisenId)
        {
            // Simulate rollback of external notifications
            _log.Info($"Rolled back notifications for Eisen ID: {eisenId}");
        }

        #endregion

        public static Props Props() => Akka.Actor.Props.Create<EisenSagaActor>();
    }
}
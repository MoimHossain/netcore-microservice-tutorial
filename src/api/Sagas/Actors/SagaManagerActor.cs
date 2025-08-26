using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using NCoreWebApp.Sagas.Messages;

namespace NCoreWebApp.Sagas.Actors
{
    /// <summary>
    /// SAGA Manager Actor that manages multiple SAGA instances
    /// </summary>
    public class SagaManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly Dictionary<Guid, IActorRef> _activeSagas = new();
        private readonly Dictionary<Guid, IActorRef> _eisenSagas = new();

        public SagaManagerActor()
        {
            Receive<StartSaga>(HandleStartSaga);
            Receive<GetSagaStatus>(HandleGetSagaStatus);
            Receive<SagaCompleted>(HandleSagaCompleted);
            Receive<SagaFailed>(HandleSagaFailed);
            Receive<CreateEisenCommand>(HandleCreateEisenCommand);
            Receive<GetActiveSagasCount>(HandleGetActiveSagasCount);
        }

        private void HandleStartSaga(StartSaga msg)
        {
            _log.Info($"Starting SAGA {msg.SagaId}");

            if (_activeSagas.ContainsKey(msg.SagaId))
            {
                _log.Warning($"SAGA {msg.SagaId} already exists");
                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, SagaStatus.InProgress));
                return;
            }

            var sagaRef = Context.ActorOf(
                SagaCoordinatorActor.Props(msg.SagaId, msg.CorrelationId),
                $"saga-{msg.SagaId}");

            _activeSagas[msg.SagaId] = sagaRef;
            sagaRef.Forward(msg);
        }

        private void HandleCreateEisenCommand(CreateEisenCommand cmd)
        {
            _log.Info($"Creating Eisen SAGA {cmd.SagaId} for: {cmd.EisenData.Title}");

            if (_eisenSagas.ContainsKey(cmd.SagaId))
            {
                _log.Warning($"Eisen SAGA {cmd.SagaId} already exists");
                return;
            }

            var eisenSagaRef = Context.ActorOf(
                EisenSagaActor.Props(),
                $"eisen-saga-{cmd.SagaId}");

            _eisenSagas[cmd.SagaId] = eisenSagaRef;
            _activeSagas[cmd.SagaId] = eisenSagaRef;

            eisenSagaRef.Forward(cmd);
        }

        private void HandleGetSagaStatus(GetSagaStatus msg)
        {
            if (_activeSagas.TryGetValue(msg.SagaId, out var sagaRef))
            {
                sagaRef.Forward(msg);
            }
            else
            {
                Sender.Tell(new SagaStatusResponse(msg.SagaId, msg.CorrelationId, SagaStatus.NotStarted));
            }
        }

        private void HandleSagaCompleted(SagaCompleted msg)
        {
            _log.Info($"SAGA {msg.SagaId} completed successfully");
            
            // Remove from active sagas after some delay to allow status queries
            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromMinutes(5),
                Self,
                new CleanupSaga(msg.SagaId),
                Self);
        }

        private void HandleSagaFailed(SagaFailed msg)
        {
            _log.Error($"SAGA {msg.SagaId} failed: {msg.Reason}");
            
            // Remove from active sagas after some delay to allow status queries
            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromMinutes(5),
                Self,
                new CleanupSaga(msg.SagaId),
                Self);
        }

        private void HandleGetActiveSagasCount(GetActiveSagasCount msg)
        {
            Sender.Tell(new ActiveSagasCountResponse(_activeSagas.Count));
        }

        protected override void PostStop()
        {
            _log.Info("SAGA Manager stopping...");
            base.PostStop();
        }

        public static Props Props() => Akka.Actor.Props.Create<SagaManagerActor>();
    }

    /// <summary>
    /// Internal message to cleanup completed/failed SAGAs
    /// </summary>
    internal record CleanupSaga(Guid SagaId);

    /// <summary>
    /// Message to get count of active SAGAs
    /// </summary>
    public record GetActiveSagasCount();

    /// <summary>
    /// Response with count of active SAGAs
    /// </summary>
    public record ActiveSagasCountResponse(int Count);
}
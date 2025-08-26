using System;
using System.Threading.Tasks;
using Akka.Actor;
using NCoreWebApp.Dtos;
using NCoreWebApp.Sagas.Messages;
using NCoreWebApp.Sagas.Actors;

namespace NCoreWebApp.Sagas.Services
{
    /// <summary>
    /// Interface for SAGA service
    /// </summary>
    public interface ISagaService
    {
        Task<Guid> StartEisenCreationSaga(EisDto eisenData);
        Task<SagaStatusResponse> GetSagaStatus(Guid sagaId);
        Task<int> GetActiveSagasCount();
    }

    /// <summary>
    /// Service for interacting with SAGA actors
    /// </summary>
    public class SagaService : ISagaService
    {
        private readonly IActorRef _sagaManager;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        public SagaService(ActorSystem actorSystem)
        {
            _sagaManager = actorSystem.ActorSelection("/user/saga-manager").ResolveOne(_timeout).Result;
        }

        public Task<Guid> StartEisenCreationSaga(EisDto eisenData)
        {
            var sagaId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var command = new CreateEisenCommand(sagaId, correlationId, eisenData);
            
            // Fire and forget - the SAGA will manage its own lifecycle
            _sagaManager.Tell(command);

            return Task.FromResult(sagaId);
        }

        public async Task<SagaStatusResponse> GetSagaStatus(Guid sagaId)
        {
            var correlationId = Guid.NewGuid();
            var query = new GetSagaStatus(sagaId, correlationId);

            try
            {
                var response = await _sagaManager.Ask<SagaStatusResponse>(query, _timeout);
                return response;
            }
            catch (Exception)
            {
                return new SagaStatusResponse(sagaId, correlationId, SagaStatus.NotStarted);
            }
        }

        public async Task<int> GetActiveSagasCount()
        {
            try
            {
                var response = await _sagaManager.Ask<ActiveSagasCountResponse>(new GetActiveSagasCount(), _timeout);
                return response.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
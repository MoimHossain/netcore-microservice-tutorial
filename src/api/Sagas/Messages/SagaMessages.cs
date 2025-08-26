using System;

namespace NCoreWebApp.Sagas.Messages
{
    /// <summary>
    /// Base interface for all SAGA messages
    /// </summary>
    public interface ISagaMessage
    {
        Guid SagaId { get; }
        Guid CorrelationId { get; }
    }

    /// <summary>
    /// Message to start a SAGA
    /// </summary>
    public record StartSaga(Guid SagaId, Guid CorrelationId, object InitialData) : ISagaMessage;

    /// <summary>
    /// Message to complete a SAGA step
    /// </summary>
    public record CompleteSagaStep(Guid SagaId, Guid CorrelationId, string StepName, object? Result = null) : ISagaMessage;

    /// <summary>
    /// Message to compensate a SAGA step (rollback)
    /// </summary>
    public record CompensateSagaStep(Guid SagaId, Guid CorrelationId, string StepName, object? Data = null) : ISagaMessage;

    /// <summary>
    /// Message indicating SAGA has completed successfully
    /// </summary>
    public record SagaCompleted(Guid SagaId, Guid CorrelationId, object? FinalResult = null) : ISagaMessage;

    /// <summary>
    /// Message indicating SAGA has failed and been compensated
    /// </summary>
    public record SagaFailed(Guid SagaId, Guid CorrelationId, string Reason, Exception? Exception = null) : ISagaMessage;

    /// <summary>
    /// Message to query SAGA status
    /// </summary>
    public record GetSagaStatus(Guid SagaId, Guid CorrelationId) : ISagaMessage;

    /// <summary>
    /// Response with SAGA status
    /// </summary>
    public record SagaStatusResponse(Guid SagaId, Guid CorrelationId, SagaStatus Status, object? Data = null) : ISagaMessage;

    /// <summary>
    /// SAGA status enumeration
    /// </summary>
    public enum SagaStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Failed,
        Compensating,
        Compensated
    }
}
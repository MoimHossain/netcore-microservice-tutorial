using System;
using NCoreWebApp.Dtos;

namespace NCoreWebApp.Sagas.Messages
{
    /// <summary>
    /// Command to create a new Eisen (project/requirement)
    /// </summary>
    public record CreateEisenCommand(Guid SagaId, Guid CorrelationId, EisDto EisenData) : ISagaMessage;

    /// <summary>
    /// Command to validate Eisen data
    /// </summary>
    public record ValidateEisenCommand(Guid SagaId, Guid CorrelationId, EisDto EisenData) : ISagaMessage;

    /// <summary>
    /// Command to persist Eisen to database
    /// </summary>
    public record PersistEisenCommand(Guid SagaId, Guid CorrelationId, EisDto EisenData) : ISagaMessage;

    /// <summary>
    /// Command to notify external systems about new Eisen
    /// </summary>
    public record NotifyEisenCreatedCommand(Guid SagaId, Guid CorrelationId, EisDto EisenData) : ISagaMessage;

    /// <summary>
    /// Event indicating Eisen validation completed
    /// </summary>
    public record EisenValidated(Guid SagaId, Guid CorrelationId, EisDto EisenData, bool IsValid, string? ValidationErrors = null) : ISagaMessage;

    /// <summary>
    /// Event indicating Eisen was persisted successfully
    /// </summary>
    public record EisenPersisted(Guid SagaId, Guid CorrelationId, EisDto EisenData, string PersistenceId) : ISagaMessage;

    /// <summary>
    /// Event indicating external notification was sent
    /// </summary>
    public record EisenNotificationSent(Guid SagaId, Guid CorrelationId, EisDto EisenData, bool Success) : ISagaMessage;

    /// <summary>
    /// Compensation command to remove persisted Eisen
    /// </summary>
    public record RemoveEisenCommand(Guid SagaId, Guid CorrelationId, string EisenId) : ISagaMessage;

    /// <summary>
    /// Compensation command to rollback external notifications
    /// </summary>
    public record RollbackEisenNotificationCommand(Guid SagaId, Guid CorrelationId, string EisenId) : ISagaMessage;
}
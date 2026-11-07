namespace Core.Domain.Entities;

public sealed record AuditRecord(
    Guid RequestId,
    string AdvisorId,
    AdvisorDecision Decision,
    string? ReviewerNotes,
    DateTimeOffset SignedAtUtc);
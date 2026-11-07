using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.UseCases;

public sealed class SubmitAdvisorDecisionUseCase(IAuditLogger auditLogger)
{
    public async Task<AuditRecord> HandleAsync(
        Guid requestId,
        string advisorId,
        AdvisorDecision decision,
        string? reviewerNotes,
        CancellationToken cancellationToken = default)
    {
        var record = new AuditRecord(
            requestId,
            advisorId,
            decision,
            reviewerNotes,
            DateTimeOffset.UtcNow);

        await auditLogger.RecordAsync(record, cancellationToken);
        return record;
    }
}
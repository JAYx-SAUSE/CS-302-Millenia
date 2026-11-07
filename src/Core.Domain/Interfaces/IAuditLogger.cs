using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IAuditLogger
{
    Task RecordAsync(AuditRecord record, CancellationToken cancellationToken = default);
}
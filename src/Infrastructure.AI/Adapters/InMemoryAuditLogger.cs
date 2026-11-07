using System.Collections.Concurrent;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Infrastructure.AI.Adapters;

public sealed class InMemoryAuditLogger : IAuditLogger
{
    private readonly ConcurrentQueue<AuditRecord> _records = new();

    public Task RecordAsync(AuditRecord record, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _records.Enqueue(record);
        return Task.CompletedTask;
    }

    public IReadOnlyCollection<AuditRecord> GetSnapshot() => _records.ToArray();
}
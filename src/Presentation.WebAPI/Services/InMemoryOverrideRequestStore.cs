using System.Collections.Concurrent;
using Core.Application.UseCases;
using Core.Domain.Entities;
using Presentation.WebAPI.Models;

namespace Presentation.WebAPI.Services;

public sealed class InMemoryOverrideRequestStore
{
    private readonly ConcurrentDictionary<Guid, OverrideRequestEntry> _requests = new();

    public OverrideRequestEntry Add(
        ProcessedOverrideRequest evaluation,
        string applicantId,
        string fileName,
        CourseRequirements requirements)
    {
        var entry = new OverrideRequestEntry(
            evaluation.RequestId,
            applicantId,
            fileName,
            requirements,
            evaluation,
            DateTimeOffset.UtcNow,
            null);
        _requests[entry.RequestId] = entry;
        return entry;
    }

    public OverrideRequestEntry? Find(Guid requestId) =>
        _requests.TryGetValue(requestId, out var entry) ? entry : null;

    public IReadOnlyCollection<OverrideRequestEntry> GetSnapshot() =>
        _requests.Values.OrderByDescending(entry => entry.SubmittedAtUtc).ToArray();

    public bool SetDecision(Guid requestId, AuditRecord decision)
    {
        while (_requests.TryGetValue(requestId, out var current))
        {
            if (current.Decision is not null)
            {
                return false;
            }

            if (_requests.TryUpdate(requestId, current with { Decision = decision }, current))
            {
                return true;
            }
        }

        return false;
    }
}
using System.Collections.Concurrent;
using Presentation.WebAPI.Models;

namespace Presentation.WebAPI.Services;

public sealed class InMemoryNoticeOutbox
{
    private readonly ConcurrentQueue<OutboxNotice> _notices = new();

    public void Add(string subject, string body, string recipient) =>
        _notices.Enqueue(new OutboxNotice(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            subject,
            body,
            recipient));

    public IReadOnlyCollection<OutboxNotice> GetSnapshot() => _notices.ToArray();
}
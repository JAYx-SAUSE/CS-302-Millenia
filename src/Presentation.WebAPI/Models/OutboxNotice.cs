namespace Presentation.WebAPI.Models;

public sealed record OutboxNotice(
    Guid NoticeId,
    DateTimeOffset CreatedAtUtc,
    string Subject,
    string Body,
    string Recipient);
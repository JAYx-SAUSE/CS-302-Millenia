using Core.Application.UseCases;
using Core.Domain.Entities;

namespace Presentation.WebAPI.Models;

public sealed record OverrideRequestEntry(
    Guid RequestId,
    string ApplicantId,
    string FileName,
    CourseRequirements Requirements,
    ProcessedOverrideRequest Evaluation,
    DateTimeOffset SubmittedAtUtc,
    AuditRecord? Decision)
{
    public string Status => Decision?.Decision.ToString() ?? "Pending";
}
using Core.Application.UseCases;
using Core.Domain.Entities;
using Infrastructure.AI.Adapters;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebAPI.Models;
using Presentation.WebAPI.Services;

namespace Presentation.WebAPI.Controllers;

[ApiController]
[Route("")]
public sealed class OverrideController(
    ProcessOverrideRequestUseCase processOverrideRequest,
    SubmitAdvisorDecisionUseCase submitAdvisorDecision,
    InMemoryOverrideRequestStore requestStore,
    InMemoryAuditLogger auditLogger,
    InMemoryNoticeOutbox noticeOutbox) : ControllerBase
{
    [HttpGet("requests")]
    public IActionResult GetRequests() => Ok(requestStore.GetSnapshot());

    [HttpGet("audit")]
    public IActionResult GetAudit() => Ok(auditLogger.GetSnapshot());

    [HttpGet("outbox")]
    public IActionResult GetOutbox() => Ok(noticeOutbox.GetSnapshot());

    [HttpPost("process")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Process(
        [FromForm] ProcessOverrideForm form,
        CancellationToken cancellationToken)
    {
        if (form.Transcript is null || form.Transcript.Length == 0)
        {
            return BadRequest("A non-empty transcript PDF is required.");
        }

        if (!string.Equals(Path.GetExtension(form.Transcript.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("The transcript must be a PDF file.");
        }

        var requirements = new CourseRequirements(
            form.TargetCourseCode.Trim(),
            form.MinimumGpa,
            ParsePrerequisites(form.RequiredPrerequisites));

        await using var transcriptStream = form.Transcript.OpenReadStream();
        try
        {
            var result = await processOverrideRequest.HandleAsync(
                transcriptStream,
                requirements,
                cancellationToken);
            var entry = requestStore.Add(
                result,
                form.ApplicantId.Trim(),
                Path.GetFileName(form.Transcript.FileName),
                requirements);
            noticeOutbox.Add(
                $"Review needed: {requirements.TargetCourseCode}",
                $"Request {result.RequestId} from {entry.ApplicantId} is ready for advisor review.",
                "SUP-001");
            return Ok(entry);
        }
        catch (InvalidDataException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("advisor-signoff")]
    public async Task<IActionResult> AdvisorSignoff(
        [FromBody] AdvisorSignoffRequest request,
        CancellationToken cancellationToken)
    {
        if (request.RequestId == Guid.Empty || request.Decision is null)
        {
            return BadRequest("A valid request ID and advisor decision are required.");
        }

        var existing = requestStore.Find(request.RequestId);
        if (existing is null)
        {
            return NotFound("The override request was not found.");
        }

        if (existing.Decision is not null)
        {
            return Conflict("This override request already has an advisor decision.");
        }

        var record = await submitAdvisorDecision.HandleAsync(
            request.RequestId,
            request.AdvisorId.Trim(),
            request.Decision.Value,
            request.ReviewerNotes,
            cancellationToken);

        if (!requestStore.SetDecision(request.RequestId, record))
        {
            return Conflict("This override request already has an advisor decision.");
        }

        noticeOutbox.Add(
            $"Override request {record.Decision}",
            $"Your request for {existing.Requirements.TargetCourseCode} was {record.Decision.ToString().ToLowerInvariant()} by {record.AdvisorId}.",
            existing.ApplicantId);

        return Ok(record);
    }

    private static IReadOnlyCollection<string> ParsePrerequisites(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
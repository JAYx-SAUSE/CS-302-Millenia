using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Domain.Services;

namespace Core.Application.UseCases;

public sealed class ProcessOverrideRequestUseCase(
    ITranscriptParserGateway transcriptParser,
    PrerequisiteValidator prerequisiteValidator)
{
    public async Task<ProcessedOverrideRequest> HandleAsync(
        Stream transcriptPdf,
        CourseRequirements requirements,
        CancellationToken cancellationToken = default)
    {
        var transcript = await transcriptParser.ParseAsync(transcriptPdf, cancellationToken);
        var recommendation = prerequisiteValidator.Evaluate(transcript, requirements);
        return new ProcessedOverrideRequest(Guid.NewGuid(), transcript, recommendation);
    }
}

public sealed record ProcessedOverrideRequest(
    Guid RequestId,
    TranscriptData Transcript,
    RecommendationPlan RecommendationPlan);
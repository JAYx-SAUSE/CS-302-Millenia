using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface ITranscriptParserGateway
{
    Task<TranscriptData> ParseAsync(Stream transcriptPdf, CancellationToken cancellationToken = default);
}
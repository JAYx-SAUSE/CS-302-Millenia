using System.Text;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Infrastructure.AI.Adapters;

public sealed class GeminiTranscriptParserAdapter : ITranscriptParserGateway
{
    public async Task<TranscriptData> ParseAsync(
        Stream transcriptPdf,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transcriptPdf);

        using var reader = new StreamReader(
            transcriptPdf,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            leaveOpen: true);
        var extractedText = await reader.ReadToEndAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            throw new InvalidDataException("The uploaded transcript is empty or contains no readable text.");
        }

        // Placeholder for Gemini structured output; no model or PDF SDK is called here.
        return new TranscriptData(
            GPA: 3.42m,
            Courses: new[]
            {
                new CompletedCourse("CS101", "A"),
                new CompletedCourse("MATH101", "B+")
            });
    }
}
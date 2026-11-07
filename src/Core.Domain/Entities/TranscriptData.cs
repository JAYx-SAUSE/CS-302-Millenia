namespace Core.Domain.Entities;

public sealed record TranscriptData(decimal GPA, IReadOnlyCollection<CompletedCourse> Courses);
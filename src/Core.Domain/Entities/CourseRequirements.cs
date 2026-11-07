namespace Core.Domain.Entities;

public sealed record CourseRequirements(
    string TargetCourseCode,
    decimal MinimumGpa,
    IReadOnlyCollection<string> RequiredPrerequisites);
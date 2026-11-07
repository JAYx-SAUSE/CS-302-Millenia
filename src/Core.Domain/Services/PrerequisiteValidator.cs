using Core.Domain.Entities;

namespace Core.Domain.Services;

public sealed class PrerequisiteValidator
{
    public RecommendationPlan Evaluate(TranscriptData transcript, CourseRequirements requirements)
    {
        ArgumentNullException.ThrowIfNull(transcript);
        ArgumentNullException.ThrowIfNull(requirements);

        if (transcript.GPA is < 0 or > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(transcript), "GPA must be between 0.0 and 4.0.");
        }

        if (requirements.MinimumGpa is < 0 or > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(requirements), "Minimum GPA must be between 0.0 and 4.0.");
        }

        var completedCourses = transcript.Courses
            .Select(course => course.CourseCode.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingPrerequisites = requirements.RequiredPrerequisites
            .Where(prerequisite => !completedCourses.Contains(prerequisite.Trim()))
            .ToArray();
        var meetsMinimumGpa = transcript.GPA >= requirements.MinimumGpa;
        var recommendation = meetsMinimumGpa && missingPrerequisites.Length == 0
            ? RecommendationStatus.RECOMMEND_APPROVE
            : RecommendationStatus.RECOMMEND_DENY;

        var summary = recommendation == RecommendationStatus.RECOMMEND_APPROVE
            ? $"The transcript meets the GPA and prerequisite requirements for {requirements.TargetCourseCode}."
            : BuildDenialSummary(meetsMinimumGpa, missingPrerequisites);

        return new RecommendationPlan(recommendation, meetsMinimumGpa, missingPrerequisites, summary);
    }

    private static string BuildDenialSummary(bool meetsMinimumGpa, IReadOnlyCollection<string> missingPrerequisites)
    {
        var reasons = new List<string>();
        if (!meetsMinimumGpa)
        {
            reasons.Add("GPA is below the minimum");
        }

        if (missingPrerequisites.Count > 0)
        {
            reasons.Add($"missing prerequisites: {string.Join(", ", missingPrerequisites)}");
        }

        return $"The transcript does not meet requirements ({string.Join("; ", reasons)}).";
    }
}
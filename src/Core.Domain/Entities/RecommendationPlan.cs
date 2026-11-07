namespace Core.Domain.Entities;

public enum RecommendationStatus
{
    RECOMMEND_APPROVE,
    RECOMMEND_DENY
}

public sealed record RecommendationPlan(
    RecommendationStatus Recommendation,
    bool MeetsMinimumGpa,
    IReadOnlyCollection<string> MissingPrerequisites,
    string Summary);
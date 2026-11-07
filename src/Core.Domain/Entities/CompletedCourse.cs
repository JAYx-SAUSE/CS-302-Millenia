namespace Core.Domain.Entities;

public sealed record CompletedCourse(string CourseCode, string? Grade = null);
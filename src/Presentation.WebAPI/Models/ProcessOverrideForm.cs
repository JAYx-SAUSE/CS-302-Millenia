using System.ComponentModel.DataAnnotations;

namespace Presentation.WebAPI.Models;

public sealed class ProcessOverrideForm
{
    [Required]
    public IFormFile? Transcript { get; init; }

    [Required]
    [StringLength(100)]
    public string ApplicantId { get; init; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string TargetCourseCode { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "4")]
    public decimal MinimumGpa { get; init; }

    public string? RequiredPrerequisites { get; init; }
}
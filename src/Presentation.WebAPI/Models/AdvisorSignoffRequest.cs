using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;

namespace Presentation.WebAPI.Models;

public sealed class AdvisorSignoffRequest
{
    [Required]
    public Guid RequestId { get; init; }

    [Required]
    [StringLength(100)]
    public string AdvisorId { get; init; } = string.Empty;

    [Required]
    public AdvisorDecision? Decision { get; init; }

    [StringLength(4000)]
    public string? ReviewerNotes { get; init; }
}
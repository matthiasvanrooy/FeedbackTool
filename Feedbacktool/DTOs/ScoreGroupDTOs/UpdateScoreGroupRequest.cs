using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.DTOs;

public class UpdateScoreGroupRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int SubjectId { get; set; }
}
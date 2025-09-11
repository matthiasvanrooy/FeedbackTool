using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.DTOs.ScoreGroupDTOs;

public class CreateScoreGroupRequest
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public int SubjectId { get; set; }
}
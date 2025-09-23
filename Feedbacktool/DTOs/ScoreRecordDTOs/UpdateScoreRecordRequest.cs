using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ExerciseItemResultDTOs;

namespace Feedbacktool.DTOs.ScoreRecordDTOs;

public class UpdateScoreRecordRequest
{
    [Required]
    public int ExerciseId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int Value { get; set; } // new score (points)
}
using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.DTOs.ScoreRecordDTOs;

public class CreateScoreRecordRequest
{
    [Required]
    public int ExerciseId { get; set; }
    
    [Required]
    public int UserId { get; set; }

    // Key = ExerciseItemId, Value = student's answer
    [Required]
    public Dictionary<int, string> Answers { get; set; } = new();
}
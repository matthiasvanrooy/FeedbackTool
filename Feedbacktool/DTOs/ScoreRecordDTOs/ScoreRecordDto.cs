using Feedbacktool.DTOs.ExerciseItemResultDTOs;

namespace Feedbacktool.DTOs.ScoreRecordDTOs;

public class ScoreRecordDto
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public int UserId { get; set; }
    public int Value { get; set; } // Total score or number of correct answers
    public DateTime RecordedAt { get; set; }
    public List<ExerciseItemResultDto> ItemResults { get; set; } = new();
}
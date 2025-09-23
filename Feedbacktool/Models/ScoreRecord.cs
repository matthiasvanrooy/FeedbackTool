namespace Feedbacktool.Models;

public class ScoreRecord
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
    public int Value { get; set; } // 0-100 percentage
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ExerciseItemResult> ItemResults { get; set; } = new List<ExerciseItemResult>();
}

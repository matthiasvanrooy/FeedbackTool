namespace Feedbacktool.Models;

public class ExerciseItemResult
{
    public int Id { get; set; }

    public int ExerciseItemId { get; set; }
    public ExerciseItem ExerciseItem { get; set; }

    public int ScoreRecordId { get; set; }
    public ScoreRecord ScoreRecord { get; set; }

    public string GivenAnswer { get; set; } = "";
    public bool IsCorrect { get; set; }
}
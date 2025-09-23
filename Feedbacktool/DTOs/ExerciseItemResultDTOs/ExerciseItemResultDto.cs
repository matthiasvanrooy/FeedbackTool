namespace Feedbacktool.DTOs.ExerciseItemResultDTOs;

public class ExerciseItemResultDto
{
    public int ExerciseItemId { get; set; }
    public string Question { get; set; } = "";
    public string GivenAnswer { get; set; } = "";
    public bool IsCorrect { get; set; }
}
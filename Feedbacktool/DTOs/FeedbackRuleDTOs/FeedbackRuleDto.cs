namespace Feedbacktool.DTOs.FeedbackRuleDTOs;

public class FeedbackRuleDto
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public int Threshold { get; set; } // 0-100%
    public string FeedbackMessage { get; set; } = "";

    // List of suggested exercises (just Id and Name for simplicity)
    public List<SimpleExerciseDto> SuggestedExercises { get; set; } = new();
}
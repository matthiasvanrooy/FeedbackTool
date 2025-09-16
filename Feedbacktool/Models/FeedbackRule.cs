namespace Feedbacktool.Models;

public class FeedbackRule
{
    public int Id { get; set; }

    // Now tied to a specific exercise
    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    // Threshold as percentage (0-100)
    public int Threshold { get; set; }

    // Message to display if score meets this threshold
    public string FeedbackMessage { get; set; } = "";

    // Optional: suggest additional exercises for practice
    public ICollection<Exercise>? SuggestedExercises { get; set; } = new List<Exercise>();
}

// EXAMPLE LOGIC IN SERVICE
// public string GetFeedback(int score, List<FeedbackRule> rules)
// {
//     var rule = rules
//         .Where(r => score <= r.Threshold)
//         .OrderByDescending(r => r.Threshold)
//         .FirstOrDefault();
//
//     return rule?.FeedbackMessage ?? "Good job!";
// }

namespace Feedbacktool.Models;

public class FeedbackRule
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int Threshold { get; set; } // e.g., 60%
    public string FeedbackMessage { get; set; } = "";
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

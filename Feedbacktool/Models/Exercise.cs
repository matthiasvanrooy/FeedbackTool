using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.Models;

public class Exercise
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = "";
    [Required]
    public string Description { get; set; } = "";
    public Category Category { get; set; }
    public int Score { get; set; }
    public int UserScore { get; set; }
    [Required]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public Exercise(int id, string name, string description, Category category, int score,  int userScore,  int subjectId)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = category;
        Score = score;
        UserScore = userScore;
        SubjectId = subjectId;
    }

    public Exercise()
    {
        
    }
}
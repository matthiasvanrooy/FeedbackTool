using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.Models;

public class Exercise
{
    public int Id { get; set; }
    [Required, MaxLength(40)]
    public string Name { get; set; } = "";
    [Required, MaxLength(40)]
    public string Description { get; set; } = "";
    public Category Category { get; set; }
    public int MaxScore { get; set; }
    [Required]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    public ICollection<ScoreRecord> ScoreRecords { get; set; } = new List<ScoreRecord>();
    public ICollection<ExerciseItem> Items { get; set; } = new List<ExerciseItem>();
    
    public Exercise(int id, string name, string description, Category category, int maxscore, int subjectId)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = category;
        MaxScore = maxscore;
        SubjectId = subjectId;
    }

    public Exercise()
    {
        
    }
}
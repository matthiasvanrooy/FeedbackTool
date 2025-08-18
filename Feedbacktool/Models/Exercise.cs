namespace Feedbacktool.Models;

public class Exercise
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public int Score { get; set; }
    public int UserScore { get; set; }
    public Subject Subject { get; set; }

    public Exercise(int id, string name, string description, Category category, int score,  int userScore,  Subject subject)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = category;
        Score = score;
        UserScore = userScore;
        Subject = subject;
    }

    public Exercise()
    {
        
    }
}
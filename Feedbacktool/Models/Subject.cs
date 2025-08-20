namespace Feedbacktool.Models;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; } = new HashSet<User>();
    public ICollection<Exercise> Exercises { get; set; } = new HashSet<Exercise>();
    public ICollection<ScoreGroup> ScoreGroups { get; set; } = new HashSet<ScoreGroup>();

    public Subject(int id, string name, ICollection<User> users, ICollection<Exercise> exercises)
    {
        Id = id;
        Name = name;
        Users = users;
        Exercises = exercises;
    }

    public Subject()
    {
        
    }
}
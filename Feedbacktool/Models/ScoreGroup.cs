namespace Feedbacktool.Models;

public class ScoreGroup : ClassGroup
{ 
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public ICollection<User> Members { get; set; } = new HashSet<User>(); // <- add this if you want easy reverse queries


    public ScoreGroup()
    {
        
    }
}
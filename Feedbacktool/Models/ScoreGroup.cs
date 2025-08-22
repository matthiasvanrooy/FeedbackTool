using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.Models;

public class ScoreGroup
{ 
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = "";
    [Required]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new HashSet<User>(); // <- add this if you want easy reverse queries

    public ScoreGroup(int id, string name, int subjectId, ICollection<User> users)
    {
        Id = id;
        Name = name;
        SubjectId = subjectId;
        Users = users;
    }
    public ScoreGroup()
    {
        
    }
}
namespace Feedbacktool.Models;

public class ClassGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new HashSet<User>();

    public ClassGroup(int id, string name, ICollection<User> users)
    {
        Id = id;
        Name = name;
        Users = users;
    }

    public ClassGroup()
    {
        
    }
}
namespace Feedbacktool.Models;

public class ClassGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; }

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
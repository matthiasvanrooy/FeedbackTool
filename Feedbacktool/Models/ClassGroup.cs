using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.Models;

public class ClassGroup
{
    public int Id { get; set; }
    [Required]
    [MaxLength(10)]
    public string Name { get; set; }  = "";
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
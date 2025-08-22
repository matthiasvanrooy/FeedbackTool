using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsTeacher { get; set; }

    public UserDTO(int id, string name, string email, bool isTeacher)
    {
        Id = id;
        Name = name;
        Email = email;
        IsTeacher = isTeacher;
    }


}
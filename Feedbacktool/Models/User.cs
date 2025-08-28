using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.Models;
public class User
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        public Role Role { get; set; }
        [Required]
        public int ClassGroupId { get; set; }

        public ClassGroup? ClassGroup { get; set; }
        public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
        public ICollection<ScoreGroup> ScoreGroups { get; set; } = new HashSet<ScoreGroup>();


        public User(int id, string name, string email, string password, Role role, int classGroupId, ICollection<Subject> subjects,  ICollection<ScoreGroup> scoreGroups)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Role = role;
            ClassGroupId = classGroupId;
            Subjects = subjects;
            ScoreGroups = scoreGroups;
        }

        // Empty constructor needed if you want frameworks like EF Core or JSON serialization to work
        public User() { }
    }
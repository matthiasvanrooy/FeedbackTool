namespace Feedbacktool.Models;
public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsTeacher { get; set; }
        public int ClassGroupId { get; set; }
        public ClassGroup ClassGroup { get; set; }
        public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
        public ICollection<ScoreGroup> ScoreGroups { get; set; } = new HashSet<ScoreGroup>();


        public User(int id, string name, string email, string password, bool isTeacher, int classGroupId, ClassGroup classGroup, ICollection<Subject> subjects,  ICollection<ScoreGroup> scoreGroups)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            IsTeacher = isTeacher;
            ClassGroupId = classGroupId;
            ClassGroup = classGroup;
            Subjects = subjects;
            ScoreGroups = scoreGroups;
        }

        // Empty constructor needed if you want frameworks like EF Core or JSON serialization to work
        public User() { }
    }
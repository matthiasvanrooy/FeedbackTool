using Feedbacktool.Models;

namespace Feedbacktool.DTOs.SubjectDTOs;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? ImageUrl { get; set; }
    public IEnumerable<User> Users { get; set; } = new HashSet<User>();
    public IEnumerable<Exercise> Exercises { get; set; } = new HashSet<Exercise>();
    public IEnumerable<ScoreGroup> ScoreGroups { get; set; } = new HashSet<ScoreGroup>();
}
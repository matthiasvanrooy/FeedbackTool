using System.ComponentModel.DataAnnotations;
using Feedbacktool.Models;

namespace Feedbacktool.DTOs.UserDTOs;

public sealed class UserDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public Role Role { get; init; }
    public ClassGroup? ClassGroup { get; set; }
    public IEnumerable<Subject> Subjects { get; set; } = new HashSet<Subject>();
    public IEnumerable<ScoreGroup> ScoreGroups { get; set; } = new HashSet<ScoreGroup>();
}
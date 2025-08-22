using Feedbacktool.Models;

namespace Feedbacktool.DTOs;

public sealed class ScoreGroupDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int SubjectId { get; init; }
    public IEnumerable<UserDto> Users { get; init; } = new List<UserDto>();
}
namespace Feedbacktool.DTOs;

public class ClassGroupDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public IEnumerable<UserDto> Users { get; init; } = new List<UserDto>();
}
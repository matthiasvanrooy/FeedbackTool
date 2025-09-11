using Feedbacktool.Models;

namespace Feedbacktool.DTOs.ExerciseDTOs;

public class ExerciseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public Category Category { get; init; }
    public int Score { get; init; } = 0;
    public int UserScore { get; init; } = 0;
    public int SubjectId { get; init; }
}
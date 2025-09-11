using Feedbacktool.Models;

namespace Feedbacktool.DTOs.ExerciseDTOs;

public class UpdateExerciseRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public Category? Category { get; init; }
    public int? Score { get; init; }
    public int? SubjectId { get; init; }
}
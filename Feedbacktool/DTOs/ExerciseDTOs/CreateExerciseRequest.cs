using System.ComponentModel.DataAnnotations;
using Feedbacktool.Models;

namespace Feedbacktool.DTOs.ExerciseDTOs;

public class CreateExerciseRequest
{
    [Required]
    public string Name { get; init; } = "";
    [Required]
    public string Description { get; init; } = "";
    [Required]
    public Category Category { get; init; }
    [Required]
    public int MaxScore { get; init; }
    [Required]
    public int SubjectId { get; init; }
}
using System.ComponentModel.DataAnnotations;
using Feedbacktool.Models;

namespace Feedbacktool.DTOs.ExerciseDTOs;

public class UpdateExerciseRequest
{
    [Required]
    public string Name { get; init; } = "";
    [Required]
    public string Description { get; init; } = "";
    [Required]
    public Category Category { get; init; }
    [Required]
    public int Score { get; init; }
    [Required]
    public int SubjectId { get; init; }
}
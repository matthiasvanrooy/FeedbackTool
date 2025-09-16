using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.DTOs.ExerciseItemDTOs;

public class CreateExerciseItemRequest
{
    [Required]
    public string Question { get; set; } = "";
    public string Answer { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.Models;

public class ExerciseItem
{
    public int Id { get; set; }

    [Required]
    public string Question { get; set; } = "";

    public string Answer { get; set; } // optional, if teacher provides

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
}

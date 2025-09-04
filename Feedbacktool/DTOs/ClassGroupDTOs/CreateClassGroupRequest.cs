using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.DTOs.ClassGroupDTOs;

public class CreateClassGroupRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

}
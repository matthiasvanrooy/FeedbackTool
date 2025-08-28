using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Feedbacktool.DTOs.SubjectDTOs;

public class UpdateSubjectRequest
{
    [Required]
    public string Name { get; set; } = "";
    public IFormFile? Image { get; set; }
}
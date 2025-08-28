using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Feedbacktool.DTOs.SubjectDTOs;

public class CreateSubjectRequest
{
    [Required]
    public string Name { get; set; } = "";
    //IFormFile for image upload
    public IFormFile Image { get; set; }
}
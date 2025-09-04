using Microsoft.AspNetCore.Http;

namespace Feedbacktool.DTOs.SubjectDTOs;

public class UpdateSubjectRequest
{
    public string? Name { get; set; } = "";
    public IFormFile? Image { get; set; }
}
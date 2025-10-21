using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.DTOs.UserDTOs;

public class UserLoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
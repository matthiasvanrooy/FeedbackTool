using System.ComponentModel.DataAnnotations;
using Feedbacktool.Models;

namespace Feedbacktool.DTOs.UserDTOs;

public class CreateUserRequest
{
    [Required] public string Name { get; init; } = "";
    [Required, EmailAddress] public string Email { get; init; } = "";
    [Required] public string Password { get; init; } = "";
    [Required] public Role Role { get; init; }
    [Required] public int ClassGroupId { get; init; }
}

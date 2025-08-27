using System.ComponentModel.DataAnnotations;
using Feedbacktool.Models;

namespace Feedbacktool.DTOs;

public class UpdateUserRequest
{
    [Required] public string Name { get; init; } = "";
    [Required, EmailAddress] public string Email { get; init; } = "";
    // Optional: if null/empty, keep existing password
    public string? Password { get; init; }
    [Required] public Role Role { get; init; }
    [Required] public int ClassGroupId { get; init; }
}
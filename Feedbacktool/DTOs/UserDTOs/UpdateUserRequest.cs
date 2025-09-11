using System.ComponentModel.DataAnnotations;
using Feedbacktool.Models;

namespace Feedbacktool.DTOs.UserDTOs;

public class UpdateUserRequest
{
    public string? Name { get; init; }
    [EmailAddress(ErrorMessage = "Email address is not valid")]
    public string? Email { get; init; }
    // Optional: if null/empty, keep existing password
    public string? Password { get; init; }
    public Role? Role { get; init; }
    public int? ClassGroupId { get; init; }
}
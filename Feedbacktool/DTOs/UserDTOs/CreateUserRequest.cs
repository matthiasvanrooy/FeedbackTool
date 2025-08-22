using Feedbacktool.Models;

namespace Feedbacktool.DTOs;

public class CreateUserRequest
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public Role Role { get; set; }            // validated by model binding
    public int ClassGroupId { get; set; }
}

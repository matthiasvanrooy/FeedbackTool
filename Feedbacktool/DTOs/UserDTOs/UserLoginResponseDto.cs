namespace Feedbacktool.DTOs.UserDTOs;
    
public sealed class UserLoginResponseDto 
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public string Role { get; init; } = "";
    public int ClassGroupId { get; init; }
    public string? Token { get; set; }
    
    public UserLoginResponseDto(Models.User user, string? token = null)
    {
        Id = user.Id;
        Name = user.Name;
        Email = user.Email;
        Role = user.Role.ToString();
        ClassGroupId = user.ClassGroupId;
        Token = token;
    }
}
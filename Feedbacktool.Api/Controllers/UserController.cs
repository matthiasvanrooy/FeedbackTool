using Feedbacktool.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ScoreGroupDTOs;
using Feedbacktool.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;

namespace Feedbacktool.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _svc;
    public UserController(UserService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken ct)
        => Ok(await _svc.GetAllUsersAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id, CancellationToken ct)
    {
        var dto = await _svc.GetUserByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{userId:int}/scoregroups")]
    public async Task<ActionResult<IEnumerable<ScoreGroupDto>>> GetUserScoreGroups(int userId, CancellationToken ct)
        => Ok(await _svc.GetUserScoreGroupsAsync(userId, ct));

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.CreateUserAsync(request, ct);
            return CreatedAtAction(nameof(GetUser), new { id = dto.Id }, dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.UpdateUserAsync(id, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
        => await _svc.DeleteUserAsync(id, ct) ? NoContent() : NotFound();
}
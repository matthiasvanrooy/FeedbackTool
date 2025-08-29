using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs;
using Feedbacktool.Services;

namespace Feedbacktool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreGroupController : ControllerBase
{
    private readonly ScoreGroupService _svc;
    public ScoreGroupController(ScoreGroupService svc) => _svc = svc;

    [HttpGet("{scoreGroupId:int}", Name = "GetScoreGroupById")]
    public async Task<ActionResult<ScoreGroupDto>> GetScoreGroup(int scoreGroupId, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(scoreGroupId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScoreGroupDto>>> GetAll(CancellationToken ct)
        => Ok(await _svc.GetAllAsync(ct));

    [HttpGet("{scoreGroupId:int}/users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(int scoreGroupId, CancellationToken ct)
        => Ok(await _svc.GetUsersAsync(scoreGroupId, ct));

    [HttpPost]
    public async Task<ActionResult<ScoreGroupDto>> Create([FromBody] CreateScoreGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.CreateAsync(request, ct);
            return CreatedAtRoute("GetScoreGroupById", new { scoreGroupId = dto.Id }, dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{scoreGroupId:int}")]
    public async Task<ActionResult<ScoreGroupDto>> Edit(int scoreGroupId, [FromBody] UpdateScoreGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.UpdateAsync(scoreGroupId, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{scoreGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> AddUser(int scoreGroupId, int userId, CancellationToken ct)
        => await _svc.AddUserAsync(scoreGroupId, userId, ct) ? NoContent() : NotFound();

    [HttpDelete("{scoreGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> RemoveUser(int scoreGroupId, int userId, CancellationToken ct)
    {
        var result = await _svc.RemoveUserAsync(scoreGroupId, userId, ct);
        return result switch
        {
            RemoveUserFromScoreGroupResult.NotFound => NotFound(),
            _ => NoContent()
        };
    }
    
    [HttpDelete("{scoreGroupId:int}")]
    public async Task<IActionResult> Delete(int scoreGroupId, CancellationToken ct)
    {
        var result = await _svc.DeleteAsync(scoreGroupId, ct);
        return result switch
        {
            DeleteScoreGroupResult.NotFound => NotFound(),
            DeleteScoreGroupResult.HasUsers => Conflict("Cannot delete ScoreGroup: users are linked. Remove users first."),
            _ => NoContent()
        };
    }
}

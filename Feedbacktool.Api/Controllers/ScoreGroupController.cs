using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.UserDTOs;
using Feedbacktool.DTOs.ScoreGroupDTOs;
using Feedbacktool.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Feedbacktool.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScoreGroupController : ControllerBase
{
    private readonly ScoreGroupService _svc;
    public ScoreGroupController(ScoreGroupService svc) => _svc = svc;

    [HttpGet("{scoreGroupId:int}", Name = "GetScoreGroupById")]
    public async Task<ActionResult<ScoreGroupDto>> GetScoreGroup(int scoreGroupId, CancellationToken ct)
    {
        var dto = await _svc.GetScoreGroupByIdAsync(scoreGroupId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScoreGroupDto>>> GetAllScoreGroups(CancellationToken ct)
        => Ok(await _svc.GetAllScoreGroupsAsync(ct));

    [HttpGet("{scoreGroupId:int}/users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersScoreGroup(int scoreGroupId, CancellationToken ct)
        => Ok(await _svc.GetUsersScoreGroupAsync(scoreGroupId, ct));

    [HttpPost]
    public async Task<ActionResult<ScoreGroupDto>> CreateScoreGroup([FromBody] CreateScoreGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.CreateScoreGroupAsync(request, ct);
            return CreatedAtRoute("GetScoreGroupById", new { scoreGroupId = dto.Id }, dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{scoreGroupId:int}")]
    public async Task<ActionResult<ScoreGroupDto>> EditScoreGroup(int scoreGroupId, [FromBody] UpdateScoreGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.UpdateScoreGroupAsync(scoreGroupId, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{scoreGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> AddUserScoreGroup(int scoreGroupId, int userId, CancellationToken ct)
        => await _svc.AddUserScoreGroupAsync(scoreGroupId, userId, ct) ? NoContent() : NotFound();

    [HttpDelete("{scoreGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> RemoveUserScoreGroup(int scoreGroupId, int userId, CancellationToken ct)
    {
        var result = await _svc.RemoveUserScoreGroupAsync(scoreGroupId, userId, ct);
        return result switch
        {
            RemoveUserFromScoreGroupResult.NotFound => NotFound(),
            _ => NoContent()
        };
    }
    
    [HttpDelete("{scoreGroupId:int}")]
    public async Task<IActionResult> DeleteScoreGroup(int scoreGroupId, CancellationToken ct)
    {
        var result = await _svc.DeleteScoreGroupAsync(scoreGroupId, ct);
        return result switch
        {
            DeleteScoreGroupResult.NotFound => NotFound(),
            DeleteScoreGroupResult.HasUsers => Conflict("Cannot delete ScoreGroup: users are linked. Remove users first."),
            _ => NoContent()
        };
    }
}

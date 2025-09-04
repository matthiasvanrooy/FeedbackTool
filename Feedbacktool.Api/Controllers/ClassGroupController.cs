using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs;
using Feedbacktool.Services;

namespace Feedbacktool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassGroupController : ControllerBase
{
    private readonly ClassGroupService _svc;

    public ClassGroupController(ClassGroupService svc) => _svc = svc;

    [HttpGet("{classGroupId:int}", Name = "GetClassGroupById")]
    public async Task<ActionResult<ClassGroupDto>> GetClassGroup(int classGroupId, CancellationToken ct)
    {
        var dto = await _svc.GetClassGroupByIdAsync(classGroupId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassGroupDto>>> GetAllClassGroups(CancellationToken ct) =>
        Ok(await _svc.GetAllClassGroupsAsync(ct));

    [HttpPost]
    public async Task<ActionResult<ClassGroupDto>> CreateClassGroup([FromBody] CreateClassGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.CreateClassGroupAsync(request, ct);
            return CreatedAtRoute("GetClassGroupById", new { classGroupId = dto.Id }, dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{classGroupId:int}")]
    public async Task<ActionResult<ClassGroupDto>> EditClassGroup(int classGroupId, [FromBody] UpdateClassGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.UpdateClassGroupAsync(classGroupId, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{classGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> AddUserClassGroup(int classGroupId, int userId, CancellationToken ct) =>
        await _svc.AssignUserClassGroupAsync(classGroupId, userId, ct) ? NoContent() : NotFound();
    
    [HttpDelete("{classGroupId:int}")]
    public async Task<IActionResult> DeleteClassGroup(int classGroupId, CancellationToken ct)
    {
        var result = await _svc.DeleteClassGroupAsync(classGroupId, ct);
        return result switch
        {
            DeleteClassGroupResult.NotFound => NotFound(),
            DeleteClassGroupResult.HasUsers => Conflict("Cannot delete ClassGroup: users are linked. Move users to another ClassGroup first."),
            _ => NoContent()
        };
    }
}

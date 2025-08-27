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

    [HttpGet("{classGroupId:int}")]
    public async Task<ActionResult<ClassGroupDto>> GetClassGroup(int classGroupId, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(classGroupId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassGroupDto>>> GetAll(CancellationToken ct) =>
        Ok(await _svc.GetAllAsync(ct));

    [HttpPost]
    public async Task<ActionResult<ClassGroupDto>> Create([FromBody] CreateClassGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.CreateAsync(request, ct);
            return CreatedAtRoute("GetClassGroupById", new { classGroupId = dto.Id }, dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{classGroupId:int}")]
    public async Task<ActionResult<ClassGroupDto>> Edit(int classGroupId, [FromBody] UpdateClassGroupRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.UpdateAsync(classGroupId, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{classGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> AddUser(int classGroupId, int userId, CancellationToken ct) =>
        await _svc.AssignUserAsync(classGroupId, userId, ct) ? NoContent() : NotFound();

    [HttpDelete("{classGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> RemoveUser(int classGroupId, int userId, CancellationToken ct)
    {
        var result = await _svc.RemoveUserAsync(classGroupId, userId, ct);
        return result switch
        {
            RemoveUserResult.NotFound => NotFound(),
            RemoveUserResult.Conflict => Conflict("A user must belong to a ClassGroup. To move them, call PUT /api/ClassGroup/{targetClassGroupId}/users/{userId}."),
            _ => NoContent()
        };
    }
}

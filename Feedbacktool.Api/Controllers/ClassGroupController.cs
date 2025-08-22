using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Feedbacktool.Models;
using Feedbacktool.DTOs;

namespace Feedbacktool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassGroupController : ControllerBase
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;
    public ClassGroupController(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET /api/ClassGroup/{classGroupId}
    [HttpGet("{classGroupId:int}", Name = "GetClassGroupById")]
    public async Task<ActionResult<ClassGroupDto>> GetClassGroup(int classGroupId)
    {
        var dto = await _db.ClassGroups
            .Where(c => c.Id == classGroupId)
            .ProjectTo<ClassGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // GET /api/ClassGroup/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ClassGroupDto>>> GetAll()
    {
        var dtos = await _db.ClassGroups
            .ProjectTo<ClassGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        return Ok(dtos);
    }

    // GET /api/ClassGroup/{classGroupId}/users
    [HttpGet("{classGroupId:int}/users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(int classGroupId)
    {
        var users = await _db.Users
            .Where(u => u.ClassGroupId == classGroupId)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        return Ok(users);
    }

    // POST /api/ClassGroup/create
    [HttpPost("create")]
    public async Task<ActionResult<ClassGroupDto>> Create([FromBody] CreateClassGroupRequest request)
    {
        if (request is null) return BadRequest("Request body is required.");

        var cg = new ClassGroup { Name = (request.Name ?? string.Empty).Trim() };
        _db.ClassGroups.Add(cg);
        await _db.SaveChangesAsync();

        var dto = _mapper.Map<ClassGroupDto>(cg);
        return CreatedAtRoute("GetClassGroupById", new { classGroupId = cg.Id }, dto);
    }

    // PUT /api/ClassGroup/edit/{classGroupId}
    [HttpPut("edit/{classGroupId:int}")]
    public async Task<ActionResult<ClassGroupDto>> Edit(int classGroupId, [FromBody] UpdateClassGroupRequest request)
    {
        if (request is null) return BadRequest("Request body is required.");

        var cg = await _db.ClassGroups
            .AsTracking()
            .SingleOrDefaultAsync(c => c.Id == classGroupId);

        if (cg is null) return NotFound();

        cg.Name = (request.Name ?? string.Empty).Trim();
        await _db.SaveChangesAsync();

        var dto = _mapper.Map<ClassGroupDto>(cg);
        return Ok(dto);
    }

    // POST /api/ClassGroup/{classGroupId}/users/{userId}
    // Assign/move the user's ClassGroupId
    [HttpPost("{classGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> AddUser(int classGroupId, int userId)
    {
        var cg = await _db.ClassGroups.FindAsync(classGroupId);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (cg is null || user is null) return NotFound();

        if (user.ClassGroupId != classGroupId)
        {
            user.ClassGroupId = classGroupId;
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    // DELETE /api/ClassGroup/{classGroupId}/users/{userId}
    [HttpDelete("{classGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> RemoveUser(int classGroupId, int userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return NotFound();

        if (user.ClassGroupId != classGroupId)
            return NotFound();

        return BadRequest("A user must belong to a ClassGroup. To move them, call POST /api/ClassGroup/{targetClassGroupId}/users/{userId}.");
    }
}
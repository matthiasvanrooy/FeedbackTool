using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Feedbacktool.Models;
using Feedbacktool.DTOs;

namespace Feedbacktool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreGroupController : ControllerBase
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;
    public ScoreGroupController(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET /api/ScoreGroup/{scoreGroupId}
    [HttpGet("{scoreGroupId:int}", Name = "GetScoreGroupById")]
    public async Task<ActionResult<ScoreGroupDto>> GetScoreGroup(int scoreGroupId)
    {
        // ProjectTo generates the proper JOINs; no Include needed
        var dto = await _db.ScoreGroups
            .Where(s => s.Id == scoreGroupId)
            .ProjectTo<ScoreGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // GET /api/ScoreGroup/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ScoreGroupDto>>> GetAll()
    {
        var dtos = await _db.ScoreGroups
            .ProjectTo<ScoreGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        return Ok(dtos);
    }

    // GET /api/ScoreGroup/{scoreGroupId}/users
    [HttpGet("{scoreGroupId:int}/users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(int scoreGroupId)
    {
        var users = await _db.Users
            .Where(u => u.ScoreGroups.Any(g => g.Id == scoreGroupId))
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        return Ok(users);
    }

    // POST /api/ScoreGroup/create
    [HttpPost("create")]
    public async Task<ActionResult<ScoreGroupDto>> Create([FromBody] CreateScoreGroupRequest request)
    {
        if (request is null) return BadRequest("Request body is required.");

        var subjectExists = await _db.Subjects.AnyAsync(s => s.Id == request.SubjectId);
        if (!subjectExists) return BadRequest($"Subject with id {request.SubjectId} does not exist.");

        var sg = new ScoreGroup
        {
            Name = (request.Name ?? string.Empty).Trim(),
            SubjectId = request.SubjectId
        };

        _db.ScoreGroups.Add(sg);
        await _db.SaveChangesAsync();

        var dto = _mapper.Map<ScoreGroupDto>(sg); // Users will be empty here
        return CreatedAtRoute("GetScoreGroupById", new { scoreGroupId = sg.Id }, dto);
    }

    // PUT /api/ScoreGroup/edit/{scoreGroupId}  (only name + subject)
    [HttpPut("edit/{scoreGroupId:int}")]
    public async Task<ActionResult<ScoreGroupDto>> Edit(
        int scoreGroupId,
        [FromBody] UpdateScoreGroupRequest request)
    {
        if (request is null) return BadRequest("Request body is required.");

        var sg = await _db.ScoreGroups
            .AsTracking()
            .SingleOrDefaultAsync(s => s.Id == scoreGroupId);

        if (sg is null) return NotFound();

        sg.Name = (request.Name ?? string.Empty).Trim();

        if (sg.SubjectId != request.SubjectId)
        {
            var exists = await _db.Subjects.AnyAsync(s => s.Id == request.SubjectId);
            if (!exists) return BadRequest($"Subject with id {request.SubjectId} does not exist.");
            sg.SubjectId = request.SubjectId;
        }

        await _db.SaveChangesAsync();

        var dto = _mapper.Map<ScoreGroupDto>(sg);
        return Ok(dto);
    }

    // POST /api/ScoreGroup/{scoreGroupId}/users/{userId}
    [HttpPost("{scoreGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> AddUser(int scoreGroupId, int userId)
    {
        var user = await _db.Users.Include(u => u.ScoreGroups).FirstOrDefaultAsync(u => u.Id == userId);
        var sg   = await _db.ScoreGroups.FirstOrDefaultAsync(g => g.Id == scoreGroupId);
        if (user is null || sg is null) return NotFound();

        if (!user.ScoreGroups.Any(g => g.Id == scoreGroupId))
            user.ScoreGroups.Add(sg);

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/ScoreGroup/{scoreGroupId}/users/{userId}
    [HttpDelete("{scoreGroupId:int}/users/{userId:int}")]
    public async Task<IActionResult> RemoveUser(int scoreGroupId, int userId)
    {
        var user = await _db.Users.Include(u => u.ScoreGroups).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return NotFound();

        var sg = user.ScoreGroups.FirstOrDefault(g => g.Id == scoreGroupId);
        if (sg is null) return NotFound();

        user.ScoreGroups.Remove(sg);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
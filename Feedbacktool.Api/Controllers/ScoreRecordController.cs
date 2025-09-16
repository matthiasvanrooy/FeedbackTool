using Feedbacktool.Api.Services;
using Feedbacktool.DTOs.ScoreRecordDTOs;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacktool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreRecordController : ControllerBase
{
    private readonly ScoreRecordService _svc;

    public ScoreRecordController(ScoreRecordService svc) => _svc = svc;

    // Student submits answers
    [HttpPost]
    public async Task<ActionResult<ScoreRecordDto>> SubmitScore(
        [FromBody] CreateScoreRecordRequest req,
        CancellationToken ct)
    {
        // Example: get userId from JWT or session
        int userId = 1; // replace with real user ID
        var dto = await _svc.SubmitScoreAsync(userId, req, ct);
        return CreatedAtAction(nameof(GetScoreById), new { scoreId = dto.Id }, dto);
    }

    [HttpGet("{scoreId:int}", Name = "GetScoreById")]
    public async Task<ActionResult<ScoreRecordDto>> GetScoreById(int scoreId, CancellationToken ct)
    {
        var dto = await _svc.GetScoresForUserAsync(scoreId, ct);
        return dto.FirstOrDefault() is {} record ? Ok(record) : NotFound();
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<ScoreRecordDto>>> GetScoresForUser(int userId, CancellationToken ct)
    {
        var scores = await _svc.GetScoresForUserAsync(userId, ct);
        return Ok(scores);
    }

    [HttpGet("exercise/{exerciseId:int}")]
    public async Task<ActionResult<IEnumerable<ScoreRecordDto>>> GetScoresForExercise(int exerciseId, CancellationToken ct)
    {
        var scores = await _svc.GetScoresForExerciseAsync(exerciseId, ct);
        return Ok(scores);
    }
    
    [HttpPut("score")]
    public async Task<ActionResult<ScoreRecordDto>> UpdateScoreRecord([FromBody] UpdateScoreRecordRequest request, CancellationToken ct)
    {
        var record = await _svc.UpdateScoreRecordAsync(request, ct);
        return record is null ? NotFound() : Ok(record);
    }

}

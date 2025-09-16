using Microsoft.AspNetCore.Mvc;
using Feedbacktool.DTOs.FeedbackRuleDTOs;
using Feedbacktool.Models;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly FeedbackService _svc;

    public FeedbackController(FeedbackService svc)
    {
        _svc = svc;
    }

    [HttpGet("exercise/{exerciseId:int}/score/{score:int}")]
    public async Task<ActionResult> GetFeedback(int exerciseId, int score)
    {
        var (message, suggestions) = await _svc.GetFeedbackAsync(exerciseId, score);
        return Ok(new { message, suggestions });
    }

    [HttpPost]
    public async Task<ActionResult<FeedbackRuleDto>> AddOrUpdateRule([FromBody] FeedbackRule rule, CancellationToken ct)
    {
        var dto = await _svc.AddOrUpdateRuleAsync(rule, ct);
        return Ok(dto);
    }

    [HttpDelete("{ruleId:int}")]
    public async Task<IActionResult> DeleteRule(int ruleId, CancellationToken ct)
    {
        var deleted = await _svc.DeleteRuleAsync(ruleId, ct);
        return deleted ? NoContent() : NotFound();
    }
}
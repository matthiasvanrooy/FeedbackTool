using Feedbacktool.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Feedbacktool;
using Feedbacktool.DTOs.FeedbackRuleDTOs;

public sealed class FeedbackService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;

    public FeedbackService(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<(string message, List<SimpleExerciseDto> suggestions)> GetFeedbackAsync(int exerciseId, int score)
    {
        var exercise = await _db.Exercises
            .Include(e => e.FeedbackRules)
            .ThenInclude(f => f.SuggestedExercises)
            .FirstOrDefaultAsync(e => e.Id == exerciseId);

        if (exercise == null)
            return ("Exercise not found", new List<SimpleExerciseDto>());

        if (!exercise.FeedbackRules.Any())
            return ("Good job!", new List<SimpleExerciseDto>());

        var percentage = (int)Math.Round((double)score / exercise.MaxScore * 100);

        var rule = exercise.FeedbackRules
            .Where(r => percentage >= r.Threshold)
            .OrderByDescending(r => r.Threshold)
            .FirstOrDefault();

        var suggestions = _mapper.Map<List<SimpleExerciseDto>>(rule?.SuggestedExercises ?? new List<Exercise>());

        return (rule?.FeedbackMessage ?? "Good job!", suggestions);
    }

    public async Task<FeedbackRuleDto> AddOrUpdateRuleAsync(FeedbackRule rule, CancellationToken ct)
    {
        if (rule.Id == 0)
            _db.FeedbackRules.Add(rule);
        else
            _db.FeedbackRules.Update(rule);

        await _db.SaveChangesAsync(ct);

        return _mapper.Map<FeedbackRuleDto>(rule);
    }

    public async Task<bool> DeleteRuleAsync(int ruleId, CancellationToken ct)
    {
        var rule = await _db.FeedbackRules.FindAsync(ruleId);
        if (rule == null) return false;

        _db.FeedbackRules.Remove(rule);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
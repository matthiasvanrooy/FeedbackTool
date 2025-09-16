using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Feedbacktool.DTOs.ScoreRecordDTOs;
using Feedbacktool.Models;

namespace Feedbacktool.Api.Services;

using Microsoft.EntityFrameworkCore;

public class ScoreRecordService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;

    public ScoreRecordService(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // Student submits answers
    public async Task<ScoreRecordDto> SubmitScoreAsync(int userId, CreateScoreRecordRequest req, CancellationToken ct)
    {
        var exercise = await _db.Exercises.Include(e => e.Items)
                                         .FirstOrDefaultAsync(e => e.Id == req.ExerciseId, ct);
        if (exercise == null) throw new ValidationException("Exercise not found.");

        var itemResults = new List<ExerciseItemResult>();
        int correctCount = 0;

        foreach (var item in exercise.Items)
        {
            var givenAnswer = req.Answers.ContainsKey(item.Id) ? req.Answers[item.Id] : "";
            bool isCorrect = string.Equals(givenAnswer.Trim(), item.Answer?.Trim(), StringComparison.OrdinalIgnoreCase);
            if (isCorrect) correctCount++;

            itemResults.Add(new ExerciseItemResult
            {
                ExerciseItemId = item.Id,
                GivenAnswer = givenAnswer,
                IsCorrect = isCorrect
            });
        }

        var scoreRecord = new ScoreRecord
        {
            ExerciseId = exercise.Id,
            UserId = userId,
            Value = correctCount, // could scale to MaxScore if you prefer
            RecordedAt = DateTime.UtcNow,
            ItemResults = itemResults
        };

        _db.ScoreRecords.Add(scoreRecord);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<ScoreRecordDto>(scoreRecord);
    }

    // Get all score records for a student
    public async Task<List<ScoreRecordDto>> GetScoresForUserAsync(int userId, CancellationToken ct)
    {
        return await _db.ScoreRecords
                        .Where(sr => sr.UserId == userId)
                        .Include(sr => sr.ItemResults)
                        .ThenInclude(ir => ir.ExerciseItem)
                        .ProjectTo<ScoreRecordDto>(_mapper.ConfigurationProvider)
                        .AsNoTracking()
                        .ToListAsync(ct);
    }

    // Optional: get all scores for an exercise (teacher dashboard)
    public async Task<List<ScoreRecordDto>> GetScoresForExerciseAsync(int exerciseId, CancellationToken ct)
    {
        return await _db.ScoreRecords
                        .Where(sr => sr.ExerciseId == exerciseId)
                        .Include(sr => sr.ItemResults)
                        .ThenInclude(ir => ir.ExerciseItem)
                        .ProjectTo<ScoreRecordDto>(_mapper.ConfigurationProvider)
                        .AsNoTracking()
                        .ToListAsync(ct);
    }
    
    public async Task<ScoreRecordDto> UpdateScoreRecordAsync(UpdateScoreRecordRequest req, CancellationToken ct)
    {
        var record = await _db.ScoreRecords
            .FirstOrDefaultAsync(r => r.ExerciseId == req.ExerciseId && r.UserId == req.UserId, ct);

        if (record == null) return null!;

        // Update the total score
        record.Value = req.Value;

        await _db.SaveChangesAsync(ct);

        // Map to DTO
        return _mapper.Map<ScoreRecordDto>(record);
    }


}

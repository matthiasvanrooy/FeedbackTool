using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ExerciseDTOs;
using Feedbacktool.Models;

namespace Feedbacktool.Services;

public enum DeleteExerciseResult
{
    Deleted,
    NotFound,
    InUse
}

public sealed class ExerciseService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;

    public ExerciseService(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ExerciseDto?> GetExerciseByIdAsync(int id, CancellationToken ct) =>
        await _db.Exercises
            .Where(e => e.Id == id)
            .ProjectTo<ExerciseDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);

    public async Task<List<ExerciseDto>> GetAllExercisesAsync(CancellationToken ct) =>
        await _db.Exercises
            .ProjectTo<ExerciseDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseRequest req, CancellationToken ct)
    {
        var name = req.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");

        var exists = await _db.Exercises.AnyAsync(e => e.Name == name, ct);
        if (exists) throw new ValidationException("An exercise with that name already exists.");

        var ex = new Exercise
        {
            Name = name,
            Description = req.Description.Trim(),
            Category = req.Category,
            Score = req.Score,
            SubjectId = req.SubjectId
        };

        _db.Exercises.Add(ex);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ExerciseDto>(ex);
    }

    public async Task<ExerciseDto?> UpdateExerciseAsync(int id, UpdateExerciseRequest req, CancellationToken ct)
    {
        var ex = await _db.Exercises.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ex is null) return null;

        var name = req.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");

        var exists = await _db.Exercises.AnyAsync(e => e.Id != id && e.Name == name, ct);
        if (exists) throw new ValidationException("An exercise with that name already exists.");

        ex.Name = name;
        ex.Description = req.Description.Trim();
        ex.Category = req.Category;
        ex.Score = req.Score;
        ex.SubjectId = req.SubjectId;

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ExerciseDto>(ex);
    }

    public async Task<DeleteExerciseResult> DeleteExerciseAsync(int exerciseId, CancellationToken ct)
    {
        // Exists?
        var exists = await _db.Exercises.AnyAsync(e => e.Id == exerciseId, ct);
        if (!exists) return DeleteExerciseResult.NotFound;

        // If exercises can be linked elsewhere, check for dependencies here
        // Example: if (_db.Submissions.Any(s => s.ExerciseId == exerciseId)) return DeleteExerciseResult.InUse;

        // Delete
        var affected = await _db.Exercises
            .Where(e => e.Id == exerciseId)
            .ExecuteDeleteAsync(ct);

        return affected > 0 ? DeleteExerciseResult.Deleted : DeleteExerciseResult.NotFound;
    }
}

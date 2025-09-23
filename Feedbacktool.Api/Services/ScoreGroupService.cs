using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ScoreGroupDTOs;
using Feedbacktool.DTOs.UserDTOs;
using Feedbacktool.Models;

namespace Feedbacktool.Api.Services;

public enum RemoveUserFromScoreGroupResult { NotFound, Success }

public enum DeleteScoreGroupResult
{
    Deleted,
    NotFound,
    HasUsers
}

public sealed class ScoreGroupService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;

    public ScoreGroupService(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ScoreGroupDto?> GetScoreGroupByIdAsync(int id, CancellationToken ct) =>
        await _db.ScoreGroups
            .Where(s => s.Id == id)
            .ProjectTo<ScoreGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);

    public async Task<List<ScoreGroupDto>> GetAllScoreGroupsAsync(CancellationToken ct) =>
        await _db.ScoreGroups
            .ProjectTo<ScoreGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<List<UserDto>> GetUsersScoreGroupAsync(int scoreGroupId, CancellationToken ct) =>
        await _db.Users
            .Where(u => u.ScoreGroups.Any(g => g.Id == scoreGroupId))
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<ScoreGroupDto> CreateScoreGroupAsync(CreateScoreGroupRequest req, CancellationToken ct)
    {
        if (req is null) throw new ValidationException("Request body is required.");

        var name = (req.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");

        var subjectExists = await _db.Subjects.AnyAsync(s => s.Id == req.SubjectId, ct);
        if (!subjectExists)
            throw new ValidationException($"Subject with id {req.SubjectId} does not exist.");

        // Optional: ensure unique (SubjectId, Name)
        var dup = await _db.ScoreGroups.AnyAsync(sg => sg.SubjectId == req.SubjectId && sg.Name == name, ct);
        if (dup) throw new ValidationException("A score group with the same name already exists for this subject.");

        var sg = new ScoreGroup
        {
            Name = name,
            SubjectId = req.SubjectId
        };

        _db.ScoreGroups.Add(sg);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<ScoreGroupDto>(sg);
    }

    public async Task<ScoreGroupDto?> UpdateScoreGroupAsync(int id, UpdateScoreGroupRequest req, CancellationToken ct)
    {
        if (req is null) throw new ValidationException("Request body is required.");

        var sg = await _db.ScoreGroups.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (sg is null) return null;

        // Update Name if provided
        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            var name = req.Name.Trim();

            // Optional: keep (SubjectId, Name) unique
            var dup = await _db.ScoreGroups.AnyAsync(x => x.Id != id && x.SubjectId == (req.SubjectId ?? sg.SubjectId) && x.Name == name, ct);
            if (dup) throw new ValidationException("A score group with the same name already exists for this subject.");

            sg.Name = name;
        }

        // Update SubjectId if provided
        if (req.SubjectId.HasValue && sg.SubjectId != req.SubjectId.Value)
        {
            var exists = await _db.Subjects.AnyAsync(s => s.Id == req.SubjectId.Value, ct);
            if (!exists) throw new ValidationException($"Subject with id {req.SubjectId.Value} does not exist.");

            sg.SubjectId = req.SubjectId.Value;
        }

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ScoreGroupDto>(sg);
    }


    public async Task<bool> AddUserScoreGroupAsync(int scoreGroupId, int userId, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.ScoreGroups)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        var sg = await _db.ScoreGroups.FirstOrDefaultAsync(g => g.Id == scoreGroupId, ct);

        if (user is null || sg is null) return false;

        if (!user.ScoreGroups.Any(g => g.Id == scoreGroupId))
            user.ScoreGroups.Add(sg);

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<RemoveUserFromScoreGroupResult> RemoveUserScoreGroupAsync(int scoreGroupId, int userId, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.ScoreGroups)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return RemoveUserFromScoreGroupResult.NotFound;

        var sg = user.ScoreGroups.FirstOrDefault(g => g.Id == scoreGroupId);
        if (sg is null) return RemoveUserFromScoreGroupResult.NotFound;

        user.ScoreGroups.Remove(sg);
        await _db.SaveChangesAsync(ct);
        return RemoveUserFromScoreGroupResult.Success;
    }
    
    public async Task<DeleteScoreGroupResult> DeleteScoreGroupAsync(int scoreGroupId, CancellationToken ct)
    {
        // Does it exist?
        var exists = await _db.ScoreGroups.AnyAsync(s => s.Id == scoreGroupId, ct);
        if (!exists) return DeleteScoreGroupResult.NotFound;

        // Any users linked?
        var hasUsers = await _db.Users
            .AnyAsync(u => u.ScoreGroups.Any(g => g.Id == scoreGroupId), ct);
        if (hasUsers) return DeleteScoreGroupResult.HasUsers;

        // Safe to delete
        var affected = await _db.ScoreGroups
            .Where(s => s.Id == scoreGroupId)
            .ExecuteDeleteAsync(ct);

        return affected > 0 ? DeleteScoreGroupResult.Deleted : DeleteScoreGroupResult.NotFound;
    }
}

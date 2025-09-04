using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ClassGroupDTOs;
using Feedbacktool.Models;

namespace Feedbacktool.Api.Services;

public enum RemoveUserResult { NotFound, Conflict, Success }

public enum DeleteClassGroupResult
{
    Deleted,
    NotFound,
    HasUsers
}

public sealed class ClassGroupService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;

    public ClassGroupService(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ClassGroupDto?> GetClassGroupByIdAsync(int id, CancellationToken ct) =>
        await _db.ClassGroups
            .Where(c => c.Id == id)
            .ProjectTo<ClassGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);

    public async Task<List<ClassGroupDto>> GetAllClassGroupsAsync(CancellationToken ct) =>
        await _db.ClassGroups
            .ProjectTo<ClassGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<ClassGroupDto> CreateClassGroupAsync(CreateClassGroupRequest req, CancellationToken ct)
    {
        var name = req.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");

        var exists = await _db.ClassGroups.AnyAsync(c => c.Name == name, ct);
        if (exists) throw new ValidationException("A class group with that name already exists.");

        var cg = new ClassGroup { Name = name };
        _db.ClassGroups.Add(cg);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ClassGroupDto>(cg);
    }

    public async Task<ClassGroupDto?> UpdateClassGroupAsync(int id, UpdateClassGroupRequest req, CancellationToken ct)
    {
        var cg = await _db.ClassGroups.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (cg is null) return null;

        var name = (req.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");

        var exists = await _db.ClassGroups.AnyAsync(c => c.Id != id && c.Name == name, ct);
        if (exists) throw new ValidationException("A class group with that name already exists.");

        cg.Name = name;
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ClassGroupDto>(cg);
    }

    public async Task<bool> AssignUserClassGroupAsync(int classGroupId, int userId, CancellationToken ct)
    {
        var cg = await _db.ClassGroups.FindAsync(new object?[] { classGroupId }, ct);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (cg is null || user is null) return false;

        if (user.ClassGroupId != classGroupId)
        {
            user.ClassGroupId = classGroupId;
            await _db.SaveChangesAsync(ct);
        }
        return true;
    }
    
    public async Task<DeleteClassGroupResult> DeleteClassGroupAsync(int classGroupId, CancellationToken ct)
    {
        // Exists?
        var exists = await _db.ClassGroups.AnyAsync(c => c.Id == classGroupId, ct);
        if (!exists) return DeleteClassGroupResult.NotFound;

        // Any users linked?
        var hasUsers = await _db.Users.AnyAsync(u => u.ClassGroupId == classGroupId, ct);
        if (hasUsers) return DeleteClassGroupResult.HasUsers;

        // Delete
        var affected = await _db.ClassGroups
            .Where(c => c.Id == classGroupId)
            .ExecuteDeleteAsync(ct);

        return affected > 0 ? DeleteClassGroupResult.Deleted : DeleteClassGroupResult.NotFound;
    }
}

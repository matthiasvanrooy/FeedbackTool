using AutoMapper;
using AutoMapper.QueryableExtensions;
using Feedbacktool.DTOs;
using Feedbacktool.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Feedbacktool.Services;

public sealed class UserService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;

    public UserService(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // -------- Queries --------

    public async Task<List<UserDto>> GetAllAsync(CancellationToken ct) =>
        await _db.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken ct) =>
        await _db.Users
            .Where(u => u.Id == id)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);

    public async Task<List<ScoreGroupDto>> GetUserScoreGroupsAsync(int userId, CancellationToken ct) =>
        await _db.ScoreGroups
            .Where(g => g.Users.Any(u => u.Id == userId))
            .ProjectTo<ScoreGroupDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    // -------- Commands --------

    public async Task<UserDto> CreateAsync(CreateUserRequest req, CancellationToken ct)
    {
        if (req is null) throw new ValidationException("Request body is required.");

        var name  = (req.Name  ?? string.Empty).Trim();
        var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
        var pwd   = (req.Password ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(name))  throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(pwd))   throw new ValidationException("Password is required.");

        var cg = await _db.ClassGroups.FindAsync(new object?[] { req.ClassGroupId }, ct);
        if (cg is null) throw new ValidationException("Class group not found.");

        var emailTaken = await _db.Users.AnyAsync(x => x.Email.ToLower() == email, ct);
        if (emailTaken) throw new ValidationException("Email already exists.");

        // NOTE: hash passwords in production
        var user = new User
        {
            Name = name,
            Email = email,
            Password = pwd,
            Role = req.Role,
            ClassGroupId = req.ClassGroupId
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest req, CancellationToken ct)
    {
        if (req is null) throw new ValidationException("Request body is required.");

        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u is null) return null;

        // Email
        var newEmail = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ValidationException("Email is required.");

        if (!string.Equals(newEmail, u.Email, StringComparison.OrdinalIgnoreCase))
        {
            var taken = await _db.Users.AnyAsync(x => x.Email.ToLower() == newEmail && x.Id != id, ct);
            if (taken) throw new ValidationException("Email already exists.");
            u.Email = newEmail;
        }

        // Scalars
        var newName = (req.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Name is required.");

        u.Name = newName;
        if (!string.IsNullOrWhiteSpace(req.Password))
        {
            // NOTE: hash in prod
            u.Password = req.Password.Trim();
        }
        u.Role = req.Role;

        // ClassGroup
        if (req.ClassGroupId != u.ClassGroupId)
        {
            var cg = await _db.ClassGroups.FindAsync(new object?[] { req.ClassGroupId }, ct);
            if (cg is null) throw new ValidationException("Class group not found.");
            u.ClassGroupId = req.ClassGroupId;
        }

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<UserDto>(u);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u is null) return false;
        _db.Users.Remove(u);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

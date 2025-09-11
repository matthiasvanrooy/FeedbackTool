using AutoMapper;
using AutoMapper.QueryableExtensions;
using Feedbacktool.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ScoreGroupDTOs;
using Feedbacktool.DTOs.UserDTOs;
using Microsoft.AspNetCore.Identity;

namespace Feedbacktool.Api.Services;

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

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct) =>
        await _db.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken ct) =>
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

    public async Task<UserDto> CreateUserAsync(CreateUserRequest req, CancellationToken ct)
    {
        var hasher = new PasswordHasher<User>();
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
            Role = req.Role,
            ClassGroupId = req.ClassGroupId
        };
        
        user.Password = hasher.HashPassword(user, pwd.Trim().ToLower());

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserRequest req, CancellationToken ct)
    {
        if (req is null) throw new ValidationException("Request body is required.");

        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u is null) return null;
        
        var hasher = new PasswordHasher<User>();

        // Update Email if provided
        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var newEmail = req.Email.Trim().ToLowerInvariant();
            if (!string.Equals(newEmail, u.Email, StringComparison.OrdinalIgnoreCase))
            {
                var taken = await _db.Users.AnyAsync(x => x.Email.ToLower() == newEmail && x.Id != id, ct);
                if (taken) throw new ValidationException("Email already exists.");
                u.Email = newEmail;
            }
        }

        // Update Name if provided
        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            u.Name = req.Name.Trim();
        }

        // Update Password if provided
        if (!string.IsNullOrWhiteSpace(req.Password))
        {
            // NOTE: hash in production
            u.Password = hasher.HashPassword(u, req.Password.Trim());
        }

        // Update Role if provided
        if (req.Role.HasValue)
        {
            u.Role = req.Role.Value;
        }

        // Update ClassGroup if provided
        if (req.ClassGroupId.HasValue && req.ClassGroupId != u.ClassGroupId)
        {
            var cg = await _db.ClassGroups.FindAsync(new object?[] { req.ClassGroupId.Value }, ct);
            if (cg is null) throw new ValidationException("Class group not found.");
            u.ClassGroupId = req.ClassGroupId.Value;
        }

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<UserDto>(u);
    }


    public async Task<bool> DeleteUserAsync(int id, CancellationToken ct)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u is null) return false;
        _db.Users.Remove(u);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

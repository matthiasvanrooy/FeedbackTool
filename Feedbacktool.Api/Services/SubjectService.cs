using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Feedbacktool.DTOs.ExerciseDTOs;
using Feedbacktool.DTOs.SubjectDTOs;
using Feedbacktool.Models;
using Microsoft.EntityFrameworkCore;

namespace Feedbacktool.Services;

public class SubjectService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;  // <-- add

    public SubjectService(ToolContext db, IMapper mapper, IWebHostEnvironment env)
    {
        _db = db;
        _mapper = mapper;
        _env = env;
    }
    
    private static readonly HashSet<string> _allowedExts = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private string GetWebRootSafe()
    {
        // Fallback if WebRootPath is null (e.g., in tests)
        return _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
    }

    private async Task<string> SaveImageAsync(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0) throw new ValidationException("Uploaded image is empty.");
        if (file.Length > 10 * 1024 * 1024) // 10MB
            throw new ValidationException("Image is too large (max 10MB).");

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext) || !_allowedExts.Contains(ext))
            throw new ValidationException("Unsupported image type. Allowed: .jpg, .jpeg, .png, .gif, .webp.");

        var uploads = Path.Combine(GetWebRootSafe(), "uploads", "subjects");
        Directory.CreateDirectory(uploads);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var absPath  = Path.Combine(uploads, fileName);

        await using var stream = File.Create(absPath);
        await file.CopyToAsync(stream, ct);

        return $"/uploads/subjects/{fileName}";
    }

    private void TryDeleteLocal(string? relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl)) return;
        // Only delete files we created under /uploads/subjects
        if (!relativeUrl.StartsWith("/uploads/subjects/", StringComparison.OrdinalIgnoreCase)) return;

        var absPath = Path.Combine(GetWebRootSafe(), relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(absPath))
        {
            try { File.Delete(absPath); } catch { /* best-effort */ }
        }
    }


    public async Task<SubjectDto?> GetSubjectByIdAsync(int id, CancellationToken ct) =>
        await _db.Subjects
            .Where(x => x.Id == id)
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);
    
    public async Task<List<SubjectDto>> GetAllSubjectsAsync(CancellationToken ct) =>
        await _db.Subjects
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);
    
    public async Task<List<ExerciseDto>> GetAllExercisesBySubjectAsync(int subjectId, CancellationToken ct) => 
        await _db.Exercises
            .Where(x => x.SubjectId == subjectId)
            .ProjectTo<ExerciseDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<int> DeleteSubjectAsync(int subjectId, CancellationToken ct) =>
        await _db.Subjects
            .Where(s => s.Id == subjectId)
            .ExecuteDeleteAsync(ct);
    
    public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectRequest req, CancellationToken ct)
    {
        if (req is null) throw new ValidationException("Request body is required.");

        var name = (req.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");

        var subject = new Subject
        {
            Name = name,
            ImageUrl = null
        };

        if (req.Image is not null && req.Image.Length > 0)
        {
            subject.ImageUrl = await SaveImageAsync(req.Image, ct);
        }

        await _db.Subjects.AddAsync(subject, ct);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectRequest req, CancellationToken ct)
    {
        if (req is null) throw new ValidationException("Request body is required.");

        var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (subject is null) return null;

        var newName = (req.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Name is required.");

        subject.Name = newName;

        if (req.Image is not null && req.Image.Length > 0)
        {
            // Save new image first; if success, delete old (best-effort)
            var newUrl = await SaveImageAsync(req.Image, ct);
            var oldUrl = subject.ImageUrl;
            subject.ImageUrl = newUrl;
            TryDeleteLocal(oldUrl);
        }

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<SubjectDto>(subject);
    }

 
    
}
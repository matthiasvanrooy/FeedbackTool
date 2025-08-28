using AutoMapper;
using AutoMapper.QueryableExtensions;
using Feedbacktool.DTOs.SubjectDTOs;
using Feedbacktool.Models;
using Microsoft.EntityFrameworkCore;

namespace Feedbacktool.Services;

public class SubjectService
{
    private readonly ToolContext _db;
    private readonly IMapper _mapper;

    public SubjectService(ToolContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(int id, CancellationToken ct) =>
        await _db.Subjects
            .Where(x => x.Id == id)
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);
    
    public async Task<List<SubjectDto>> GetAllAsync(CancellationToken ct) =>
        await _db.Subjects
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(ct);
    
    //public async Task<List<ExerciseDto>> GetAllExercisesAsync(CancellationToken ct) =>
}
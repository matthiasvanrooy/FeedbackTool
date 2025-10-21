using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ExerciseDTOs;
using Feedbacktool.DTOs.SubjectDTOs;
using Feedbacktool.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacktool.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly SubjectService _svc;

    public SubjectsController(SubjectService svc)
    {
        _svc = svc;
    }

    // GET: api/subjects
    [HttpGet]
    public async Task<ActionResult<List<SubjectDto>>> GetAllSubjects(CancellationToken ct)
    {
        var items = await _svc.GetAllSubjectsAsync(ct);
        return Ok(items);
    }

    // GET: api/subjects/{id}
    [HttpGet("{id:int}", Name = nameof(GetSubjectById))]
    public async Task<ActionResult<SubjectDto>> GetSubjectById(int id, CancellationToken ct)
    {
        var item = await _svc.GetSubjectByIdAsync(id, ct);
        if (item is null) return NotFound();
        return Ok(item);
    }

    // GET: api/subjects/{id}/exercises
    [HttpGet("{id:int}/exercises")]
    public async Task<ActionResult<List<ExerciseDto>>> GetExercisesBySubject(int id, CancellationToken ct)
    {
        // Optionally: check Subject exists first; skipping for brevity
        var items = await _svc.GetAllExercisesBySubjectAsync(id, ct);
        return Ok(items);
    }

    // POST: api/subjects
    // Accepts multipart/form-data because of IFormFile
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<SubjectDto>> CreateSubject([FromForm] CreateSubjectRequest req, CancellationToken ct)
    {
        try
        {
            var created = await _svc.CreateSubjectAsync(req, ct);
            return CreatedAtRoute(nameof(GetSubjectById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    // PUT: api/subjects/{id}
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<SubjectDto>> UpdateSubject(int id, [FromForm] UpdateSubjectRequest req, CancellationToken ct)
    {
        try
        {
            var updated = await _svc.UpdateSubjectAsync(id, req, ct);
            if (updated is null) return NotFound();
            return Ok(updated);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    // DELETE: api/subjects/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubject(int id, CancellationToken ct)
    {
        try
        {
            var affected = await _svc.DeleteSubjectAsync(id, ct); // <- after renaming in service
            if (affected == 0) return NotFound();
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    // Helper to return RFC 7807 ProblemDetails for ValidationException
    private ActionResult ValidationProblem(string message)
        => Problem(title: "Validation failed", detail: message, statusCode: StatusCodes.Status400BadRequest);
}

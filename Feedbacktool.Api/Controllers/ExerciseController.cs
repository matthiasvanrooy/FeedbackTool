using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Feedbacktool.DTOs.ExerciseDTOs;
using Feedbacktool.Services;

namespace Feedbacktool.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExerciseController : ControllerBase
{
    private readonly ExerciseService _svc;

    public ExerciseController(ExerciseService svc) => _svc = svc;

    [HttpGet("{exerciseId:int}", Name = "GetExerciseById")]
    public async Task<ActionResult<ExerciseDto>> GetExercise(int exerciseId, CancellationToken ct)
    {
        var dto = await _svc.GetExerciseByIdAsync(exerciseId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAllExercises(CancellationToken ct) =>
        Ok(await _svc.GetAllExercisesAsync(ct));

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> CreateExercise([FromBody] CreateExerciseRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.CreateExerciseAsync(request, ct);
            return CreatedAtRoute("GetExerciseById", new { exerciseId = dto.Id }, dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpPut("{exerciseId:int}")]
    public async Task<ActionResult<ExerciseDto>> EditExercise(int exerciseId, [FromBody] UpdateExerciseRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.UpdateExerciseAsync(exerciseId, request, ct);
            return dto is null ? NotFound() : Ok(dto);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    [HttpDelete("{exerciseId:int}")]
    public async Task<IActionResult> DeleteExercise(int exerciseId, CancellationToken ct)
    {
        var result = await _svc.DeleteExerciseAsync(exerciseId, ct);
        return result switch
        {
            DeleteExerciseResult.NotFound => NotFound(),
            DeleteExerciseResult.InUse => Conflict("Cannot delete Exercise: it is in use."),
            _ => NoContent()
        };
    }
}

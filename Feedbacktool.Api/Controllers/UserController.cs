using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Feedbacktool.Models;          // User, ClassGroup, etc.

namespace Feedbacktool.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ToolContext _db;
        public UsersController(ToolContext db) => _db = db;

        // DTOs - never expose Password
        public record UserDto(int Id, string Name, string Email, bool IsTeacher, int ClassGroupId);
        public record CreateUserDto(string Name, string Email, string Password, bool IsTeacher, int ClassGroupId);
        public record UpdateUserDto(string? Name, string? Email, string? Password, bool? IsTeacher, int? ClassGroupId);

        private static UserDto ToDto(User u) => new(u.Id, u.Name, u.Email, u.IsTeacher, u.ClassGroupId);

        /// <summary>
        /// GET api/users
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
        {
            var data = await _db.Users
                .AsNoTracking()
                .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsTeacher, u.ClassGroupId))
                .ToListAsync(ct);
            return Ok(data);
        }

        /// <summary>
        /// GET api/users/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDto>> Get(int id, CancellationToken ct)
        {
            var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            return u is null ? NotFound() : Ok(ToDto(u));
        }

        /// <summary>
        /// POST api/users
        /// Creates a user. ClassGroupId is required and must exist.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto body, CancellationToken ct)
        {
            // Email uniqueness
            var emailExists = await _db.Users.AnyAsync(x => x.Email == body.Email, ct);
            if (emailExists)
            {
                ModelState.AddModelError("email", "Email already exists.");
                return ValidationProblem(ModelState);
            }

            // Required ClassGroup must exist
            var cg = await _db.ClassGroups.FindAsync(new object?[] { body.ClassGroupId }, ct);
            if (cg is null)
            {
                ModelState.AddModelError("classGroupId", "Class group not found.");
                return ValidationProblem(ModelState);
            }

            var user = new User
            {
                Name = body.Name,
                Email = body.Email,
                Password = body.Password, // TODO: hash in production
                IsTeacher = body.IsTeacher,
                ClassGroupId = body.ClassGroupId,
                ClassGroup = cg
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(Get), new { id = user.Id }, ToDto(user));
        }

        /// <summary>
        /// PUT api/users/{id}
        /// Updates an existing user. If ClassGroupId is provided, it must exist.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto body, CancellationToken ct)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (u is null) return NotFound();

            // Email change → check uniqueness
            if (body.Email is not null && !string.Equals(body.Email, u.Email, System.StringComparison.OrdinalIgnoreCase))
            {
                var taken = await _db.Users.AnyAsync(x => x.Email == body.Email && x.Id != id, ct);
                if (taken)
                {
                    ModelState.AddModelError("email", "Email already exists.");
                    return ValidationProblem(ModelState);
                }
                u.Email = body.Email;
            }

            if (body.Name is not null) u.Name = body.Name;
            if (body.Password is not null) u.Password = body.Password; // TODO: hash
            if (body.IsTeacher.HasValue) u.IsTeacher = body.IsTeacher.Value;

            // Optional change of ClassGroup
            if (body.ClassGroupId.HasValue)
            {
                var cg = await _db.ClassGroups.FindAsync(new object?[] { body.ClassGroupId.Value }, ct);
                if (cg is null)
                {
                    ModelState.AddModelError("classGroupId", "Class group not found.");
                    return ValidationProblem(ModelState);
                }
                u.ClassGroupId = body.ClassGroupId.Value;
                u.ClassGroup = cg;
            }

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        /// <summary>
        /// DELETE api/users/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (u is null) return NotFound();

            _db.Users.Remove(u);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}

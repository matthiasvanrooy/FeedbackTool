using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Feedbacktool.Models;   // User, ClassGroup, etc.

namespace Feedbacktool.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ToolContext _db;
        public UserController(ToolContext db) => _db = db;

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _db.Users
                .Include(u => u.ClassGroup)
                .AsNoTracking()
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _db.Users
                .Include(u => u.ClassGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return user is null ? NotFound() : Ok(user);
        }
        
        [HttpGet("{userId:int}/scoregroups")]
        public async Task<ActionResult<IEnumerable<ScoreGroup>>> GetUserScoreGroups(int userId)
        {
            var groups = await _db.ScoreGroups
                .Where(g => g.Users.Any(u => u.Id == userId))  // requires ScoreGroup.Users nav (see note)
                .Include(s => s.Users)
                .AsNoTracking()
                .ToListAsync();

            return Ok(groups);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            // Required ClassGroup must exist
            var cg = await _db.ClassGroups.FindAsync(user.ClassGroupId);
            if (cg is null)
            {
                ModelState.AddModelError("classGroupId", "Class group not found.");
                return ValidationProblem(ModelState);
            }

            // Email uniqueness
            var emailTaken = await _db.Users.AnyAsync(x => x.Email == user.Email);
            if (emailTaken)
            {
                ModelState.AddModelError("email", "Email already exists.");
                return ValidationProblem(ModelState);
            }

            // NOTE: hash passwords in production
            user.ClassGroup = cg;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u is null) return NotFound();

            // Email change → check uniqueness
            if (!string.Equals(user.Email, u.Email, System.StringComparison.OrdinalIgnoreCase))
            {
                var taken = await _db.Users.AnyAsync(x => x.Email == user.Email && x.Id != id);
                if (taken)
                {
                    ModelState.AddModelError("email", "Email already exists.");
                    return ValidationProblem(ModelState);
                }
                u.Email = user.Email;
            }

            // Update scalar fields
            u.Name = user.Name;
            u.Password = user.Password; // NOTE: hash in prod
            u.IsTeacher = user.IsTeacher;

            // Update required ClassGroup (must exist)
            if (user.ClassGroupId != u.ClassGroupId)
            {
                var cg = await _db.ClassGroups.FindAsync(user.ClassGroupId);
                if (cg is null)
                {
                    ModelState.AddModelError("classGroupId", "Class group not found.");
                    return ValidationProblem(ModelState);
                }
                u.ClassGroupId = user.ClassGroupId;
                u.ClassGroup = cg;
            }

            // TODO: handle Subjects/ScoreGroups sync if you post them on update

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u is null) return NotFound();

            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

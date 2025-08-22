using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Feedbacktool.Models;
using Feedbacktool.DTOs;

namespace Feedbacktool.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreGroupController : ControllerBase
    {
        private readonly ToolContext _db;
        public ScoreGroupController(ToolContext db) => _db = db;
        
        //GET
        [HttpGet("{scoreGroupId:int}")]
        public async Task<ActionResult<ScoreGroup>> GetScoreGroup(int scoreGroupId)
        {
            var sg = await _db.ScoreGroups
                .Include(s => s.Users)
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == scoreGroupId);
    
            if (sg is null) return NotFound();
    
            return sg;
        }
    
        [HttpGet("/all")]
        public async Task<ActionResult<IEnumerable<ScoreGroup>>> GetAll()
        {
            var sgs = await _db.ScoreGroups
                .Include(sg => sg.Users)
                .ToListAsync();
            
            return sgs;
        }
    
        // POST /api/scoregroups/{scoreGroupId}/users/{userId}
        [HttpPost("{scoreGroupId:int}/users/{userId:int}")]
        public async Task<IActionResult> AddUser(int scoreGroupId, int userId)
        {
            var user = await _db.Users.Include(u => u.ScoreGroups).FirstOrDefaultAsync(u => u.Id == userId);
            var sg   = await _db.ScoreGroups.FirstOrDefaultAsync(g => g.Id == scoreGroupId);
            if (user is null || sg is null) return NotFound();
    
            if (!user.ScoreGroups.Any(g => g.Id == scoreGroupId))
                user.ScoreGroups.Add(sg);
    
            await _db.SaveChangesAsync();
            return NoContent(); // idempotent
        }
    
        // DELETE /api/scoregroups/{scoreGroupId}/users/{userId}
        [HttpDelete("{scoreGroupId:int}/users/{userId:int}")]
        public async Task<IActionResult> RemoveUser(int scoreGroupId, int userId)
        {
            var user = await _db.Users.Include(u => u.ScoreGroups).FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return NotFound();
    
            var sg = user.ScoreGroups.FirstOrDefault(g => g.Id == scoreGroupId);
            if (sg is null) return NotFound();
    
            user.ScoreGroups.Remove(sg);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    
        // GET /api/scoregroups/{scoreGroupId}/users
        [HttpGet("{scoreGroupId:int}/users")]
        public async Task<ActionResult<IEnumerable<User>>> GetMembers(int scoreGroupId)
        {
            var users = await _db.Users
                .Where(u => u.ScoreGroups.Any(g => g.Id == scoreGroupId))
                .AsNoTracking()
                .ToListAsync();
    
            return Ok(users);
        }
    }
}

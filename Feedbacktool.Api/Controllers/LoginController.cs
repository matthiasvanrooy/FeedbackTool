using Microsoft.AspNetCore.Mvc;
using Feedbacktool.DTOs.UserDTOs;
using Feedbacktool.Api.Services;

namespace Feedbacktool.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login data.");

            var (user, token) = await _loginService.ValidateUserAsync(request.Email, request.Password);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            // Set HTTP-only cookie
            Response.Cookies.Append("jwt", token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // true if using HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(6)
            });

            // Return user info without token
            var response = new UserLoginResponseDto(user, token: null); 
            return Ok(response);
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Logged out successfully" });
        }
    }


}
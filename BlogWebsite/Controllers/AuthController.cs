using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Users user)
        {
            try
            {
                // Check if phone number already exists
                var existing = await _userRepository.Login(user.phoneNo, user.Role);
                if (existing != null)
                    return BadRequest("Phone number already registered with this role.");

                var id = await _userRepository.Register(user);
                return Ok(new { message = "User registered successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Users loginData)
        {
            var user = await _userRepository.Login(loginData.phoneNo, loginData.Role);
            if (user == null)
                return Unauthorized("Invalid phone number or role");

            return Ok(new { message = "Login successful", user });
        }
    }
}

using BlogWebsite.DTOs;
using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using BlogWebsite.Services;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;

        public AuthController(IUserRepository userRepository, IConfiguration configuration, JwtService jwtService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtService = jwtService;
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

            var token = _jwtService.GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful",
                token,
                user
            });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _userRepository.GetRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logoutDto)
        {            
            var result = await _userRepository.Logout(logoutDto.userId);

            if (!result)
                return NotFound("User not found");

            return Ok(new { message = "Logout successful" });
        }
    }
}

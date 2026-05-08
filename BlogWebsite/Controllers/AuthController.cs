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
        private readonly JwtService _jwtService;

        public AuthController(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
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

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var storedToken = await _userRepository.GetRefreshTokenAsync(refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }

            var user = await _userRepository.GetUserByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return Unauthorized("Invalid refresh token");
            }
            // rotate token
            await _userRepository.RevokeTokenAsync(refreshToken);

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            await _userRepository.SaveRefreshTokenAsync(newRefreshToken, user.Id);

            var newAccessToken = _jwtService.GenerateJwtToken(user);


            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginData)
        {
            var user = await _userRepository.Login(loginData.phoneNo, loginData.Role);

            if (user == null)
                return Unauthorized("Invalid phone number or role");

            var accessToken = _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _userRepository.SaveRefreshTokenAsync(refreshToken, user.Id);


            return Ok(new
            {
                message = "Login successful",
                accessToken, refreshToken,
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
            // update user inactive
            var result = await _userRepository.Logout(logoutDto.userId);

            if (!result)
                return NotFound("User not found");

            // revoke refresh token
            if (!string.IsNullOrEmpty(logoutDto.refreshToken))
            {
                await _userRepository.RevokeTokenAsync(logoutDto.refreshToken);
            }

            return Ok(new { message = "Logout successful" });
        }
    }
}

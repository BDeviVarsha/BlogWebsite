using BlogWebsite.DTOs;
using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _repo;

        public AdminController(IAdminRepository repo)
        {
            _repo = repo;
        }

        // Get all posts
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _repo.GetAllPosts();
            return Ok(posts);
        }

        // Delete post
        [HttpPut("posts/{id}/delete")]
        public async Task<IActionResult> DeletePost(int id, [FromBody] DeletePostDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Reason))
                return BadRequest(new { message = "Delete reason is required" });

            var result = await _repo.DeletePost(id, dto.Reason);

            if (!result)
                return NotFound(new { message = "Post not found" });

            return Ok(new { message = "Post deleted successfully" });
        }

        // Reject post
        [HttpPut("posts/{id}/reject")]
        public async Task<IActionResult> RejectPost(int id, [FromBody] RejectedReason reason)
        {
            if (reason == null || string.IsNullOrEmpty(reason.Reason))
                return BadRequest(new { message = "Rejection reason is required" });

            var result = await _repo.RejectPost(id, reason.Reason);

            if (!result)
                return NotFound(new { message = "Post not found" });

            return Ok(new { message = "Post rejected" });
        }

        // Approve post
        [HttpPut("posts/{id}/approve")]
        public async Task<IActionResult> ApprovePost(int id)
        {
            var result = await _repo.ApprovePost(id);

            if (!result)
                return NotFound(new { message = "Post not found" });

            return Ok(new { message = "Post approved" });
        }
        [HttpPost("announcements")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] Announcement announcement)
        {
            string name = "";
            if (User.Identity != null && User.Identity.IsAuthenticated) 
            {
                name = User.Identity.Name;
            }
            announcement.Created_by = name;
            if (announcement == null)
                return BadRequest(new { message = "Invalid announcement data" });

            var result = await _repo.CreateAnnouncement(announcement);

            if (result <= 0)
                return StatusCode(500, new { message = "Failed to create announcement" });

            return Ok(new { message = "Announcement created successfully" });
        }
    }
}
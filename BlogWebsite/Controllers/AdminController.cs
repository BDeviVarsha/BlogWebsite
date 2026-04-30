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
        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await _repo.DeletePost(id);

            if (!result)
                return NotFound("Post not found");

            return Ok(new { message = "Post deleted successfully" });
        }

        // Reject post
        [HttpPut("posts/{id}/reject")]
        public async Task<IActionResult> RejectPost(int id)
        {
            var result = await _repo.RejectPost(id);

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
    }
}

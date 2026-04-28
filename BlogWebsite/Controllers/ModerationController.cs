using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModerationController : ControllerBase
    {
        private readonly IModerationRepository _moderationRepository;

        public ModerationController(IModerationRepository moderationRepository)
        {
            _moderationRepository = moderationRepository;
        }

        // Get all pending anonymous posts
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPosts()
        {
            var posts = await _moderationRepository.GetPendingAnonymousPostsAsync();
            return Ok(posts);
        }

        // Approve a post
        [HttpPost("approve/{postId}")]
        public async Task<IActionResult> ApprovePost(int postId)
        {
            var success = await _moderationRepository.UpdatePostStatusAsync(postId, "Approved");
            if (!success) return NotFound("Post not found");
            return Ok(new { message = "Post approved" });
        }

        // Reject a post
        [HttpPost("reject/{postId}")]
        public async Task<IActionResult> RejectPost(int postId)
        {
            var success = await _moderationRepository.UpdatePostStatusAsync(postId, "Rejected");
            if (!success) return NotFound("Post not found");
            return Ok(new { message = "Post rejected" });
        }
    }
}

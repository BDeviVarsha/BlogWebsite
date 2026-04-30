using BlogWebsite.DTOs;
using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Moderator")]
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
            var success = await _moderationRepository.ApprovePostAsync(postId);
            if (!success) return NotFound("Post not found");
            return Ok(new { message = "Post approved" });
        }
        [HttpPost("reject/{postId}")]
        public async Task<IActionResult> RejectPost(int postId, [FromBody] RejectedReason request)
        {
            var success = await _moderationRepository.RejectPostAsync(postId, request.Reason);

            if (!success)
                return NotFound(new { message = "Post not found" });

            return Ok(new { message = "Post rejected" });
        }
    }
}

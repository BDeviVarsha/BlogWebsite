using BlogWebsite.DTOs;
using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddPostController : ControllerBase
    {
        private readonly IAddPostRepository _addPostRepository;

        public AddPostController(IAddPostRepository addPostRepository)
        {
            _addPostRepository = addPostRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PostDto postdto)
        {
            string username = "Anonymous";
            int? userId = null;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                username = User.Identity.Name;
                userId = int.Parse(User.FindFirst("id")?.Value);
            }

            postdto.Name = username;
            postdto.UserId = userId;
            var postId = await _addPostRepository.Create(postdto);
            return Ok(postId);
        }
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(int postId, [FromForm] PostDto postdto)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value);
            postdto.UserId = userId;
            var updated = await _addPostRepository.Update(postId, postdto);
            if (updated == null)
                return NotFound(new { message = "Post not found or you don't have permission to edit it" });
            return Ok(updated);
        }
    }
}

using BlogWebsite.DTOs;
using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            int? userId=null;

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
    }
}

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
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PostDto postdto)
        {
            string username = "Anonymous";

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                username = User.Identity.Name; // comes from JWT
            }

            postdto.Name = username;

            var postId = await _addPostRepository.Create(postdto);
            return Ok(postId);
        }

    }
}

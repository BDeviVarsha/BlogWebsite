using BlogWebsite.DTOs;
using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Http;
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
            var postId = await _addPostRepository.Create(postdto);
            return Ok(postId);
            
        }

    }
}

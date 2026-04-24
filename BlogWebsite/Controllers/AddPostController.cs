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
            //var postId = await _addPostRepository.Create(postdto);
            //return Ok(postId);
            string folder = "uploads";
            var fileName = $"{Guid.NewGuid()}_{postdto.File.FileName}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder, fileName);

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await postdto.File.CopyToAsync(stream);
            }

            // Save post info to database
            var post = new Post
            {
                Name = postdto.Name,
                Title = postdto.Title,
                Description = postdto.Description,
                FilePath = $"/{folder}/{fileName}", // accessible via HTTP
                //MediaType = file.ContentType.Split('/')[0] // "image", "video", or "audio"
            };

            // Save `post` to your database here (e.g., via Dapper or EF Core)
            var postId = await _addPostRepository.Create(postdto);

            return Ok(new { message = "Post created", postId, filePath = post.FilePath });
        }

    }
}

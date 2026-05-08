using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeRepository _homeRepository;
        private readonly SftpService _sftpService;

        public HomeController(IHomeRepository homeRepository, SftpService sftpService)
        {
            _homeRepository = homeRepository;
            _sftpService = sftpService;
        }
        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts(int page = 1, int limit = 10)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var posts = await _homeRepository.GetAllPosts(page, limit);
            var total = await _homeRepository.GetTotalPostsAsync();

            return Ok(new
            {
                posts, total
            });
        }

        //[HttpGet("file/{fileName}")]
        //public async Task<IActionResult> GetFile(string fileName)
        //{
        //    // Basic path traversal guard
        //    if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
        //        return BadRequest("Invalid file name.");

        //    var stream = await _sftpService.DownloadFileAsync(fileName);

        //    if (stream == null)
        //        return NotFound(new { message = $"File '{fileName}' not found on remote server." });

        //    var contentType = GetContentType(fileName);
        //    return File(stream, contentType, enableRangeProcessing: true);
        //}

        //private static string GetContentType(string fileName)
        //{
        //    var ext = Path.GetExtension(fileName).ToLowerInvariant();
        //    return ext switch
        //    {
        //        ".jpg" or ".jpeg" => "image/jpeg",
        //        ".png" => "image/png",
        //        ".mp3" => "audio/mpeg",
        //        ".mp4" => "video/mp4",
        //        _ => "application/octet-stream"
        //    };
        //}
        //[HttpGet("file/{fileName}")]
        //public IActionResult GetFile(string fileName)
        //{
        //    var stream = _sftpService.DownloadFile(fileName);
        //    if (stream == null)
        //        return NotFound();

        //    var contentType = fileName.EndsWith(".mp3") ? "audio/mpeg" :
        //                      fileName.EndsWith(".png") ? "image/png" :
        //                      fileName.EndsWith(".jpg") ? "image/jpeg" :
        //                      fileName.EndsWith(".mp4") ? "video/mp4" :
        //                      "application/octet-stream";

        //    return File(stream, contentType, fileName);
        //}

        [HttpGet("my-posts")]
        public async Task<IActionResult> GetMyPosts()
        {
            var userId = User.FindFirst("id")?.Value;

            var posts = await _homeRepository.GetPostsByUserId(int.Parse(userId));

            return Ok(posts);
        }

        [HttpDelete("my-posts/{postId}")]
        public async Task<IActionResult> DeleteMyPost(int postId)
        {
            var userId = User.FindFirst("id")?.Value;
            var success = await _homeRepository.DeletePostAsync(postId, int.Parse(userId));
            if (!success)
                return NotFound(new { message = "Post not found or you don't have permission to delete it" });
            return Ok(new { message = "Post deleted successfully" });
        }

        [HttpPut("my-posts/{postId}")]
        public async Task<IActionResult> EditMyPost(int postId, [FromBody] Post post)
        {
            var userId = User.FindFirst("id")?.Value;
            post.UserId = int.Parse(userId);
            var updatedPost = await _homeRepository.EditPostById(postId, post);
            if (updatedPost == null)
                return NotFound(new { message = "Post not found or you don't have permission to edit it" });
            return Ok(updatedPost);
        }

        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncement()
        {
            var result = await _homeRepository.GetAnnouncement();
            return Ok(result);
        }
    }
}

using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Get()
        {
            var posts = await _homeRepository.GetAllPosts();
            return Ok(posts);
        }
        
        [HttpGet("file/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            var stream = _sftpService.DownloadFile(fileName);
            if (stream == null)
                return NotFound();

            var contentType = fileName.EndsWith(".mp3") ? "audio/mpeg" :
                              fileName.EndsWith(".png") ? "image/png" :
                              fileName.EndsWith(".jpg") ? "image/jpeg" :
                              fileName.EndsWith(".mp4") ? "video/mp4" :
                              "application/octet-stream";

            return File(stream, contentType, fileName);
        }
    }
}

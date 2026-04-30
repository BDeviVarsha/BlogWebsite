using BlogWebsite.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public ChatController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] int user1, [FromQuery] int user2)
        {
            var messages = await _messageRepository.GetMessagesAsync(user1, user2);
            return Ok(messages);
        }
    }
}

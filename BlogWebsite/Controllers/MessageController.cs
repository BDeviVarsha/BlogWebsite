//using BlogWebsite.Models;
//using Dapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Data;

//namespace BlogWebsite.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MessageController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;

//        public MessageController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        private IDbConnection CreateConnection()
//        {
//            return new Npgsql.NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//        }

//        // Get all users
//        [HttpGet("users")]
//        public async Task<ActionResult<List<Users>>> GetUsers()
//        {
//            using var connection = CreateConnection();
//            var users = await connection.QueryAsync<Users>("SELECT Id, Name,\"phoneNo\", role, is_active FROM \"Users\"");
//            return Ok(users);
//        }

//        // Get messages between two users
//        [HttpGet("messages/{userId}/{receiverId}")]
//        public async Task<ActionResult<List<Message>>> GetMessages(int userId, int receiverId)
//        {
//            using var connection = CreateConnection();
//            var query = @"
//            SELECT Id, sender_id, receiver_id, Content, Timestamp, Seen
//            FROM Messages
//            WHERE (sender_id = @UserId AND receiver_id = @ReceiverId)
//               OR (sender_id = @ReceiverId AND receiver_id = @UserId)
//            ORDER BY Timestamp";

//            var messages = await connection.QueryAsync<Message>(query, new { UserId = userId, ReceiverId = receiverId });
//            return Ok(messages);
//        }

//        // Insert a new message
//        [HttpPost("messages")]
//        public async Task<ActionResult> SendMessage([FromBody] Message message)
//        {
//            using var connection = CreateConnection();
//            var query = @"
//            INSERT INTO Messages (sender_id, receiver_id, Content, Timestamp, Seen)
//            VALUES (@SenderId, @ReceiverId, @Content, @Timestamp, @Seen)";

//            await connection.ExecuteAsync(query, message);
//            return Ok();
//        }
//    }
//}

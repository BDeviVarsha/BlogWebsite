using BlogWebsite.Models;
using Dapper;
using Npgsql;

namespace BlogWebsite.Repositories.Implementation
{
    public class MessageRepository
    {
        private readonly string _connectionString;

        public MessageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Message>> GetMessages(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @"SELECT * FROM messages 
                      WHERE sender_id = @userId OR receiver_id = @userId 
                      ORDER BY timestamp";
            return await connection.QueryAsync<Message>(query, new { userId });
        }

        public async Task<Message> SendMessage(Message message)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = @"INSERT INTO messages(sender_id, receiver_id, content) 
                      VALUES (@SenderId, @ReceiverId, @Content) 
                      RETURNING *";
            return await connection.QuerySingleAsync<Message>(query, message);
        }
    }
}

using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
using Npgsql;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class MessageRepository:IMessageRepository
    {
        //private readonly string _connectionString;

        //public MessageRepository(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        //public async Task<IEnumerable<Message>> GetMessages(int userId)
        //{
        //    using var connection = new NpgsqlConnection(_connectionString);
        //    var query = @"SELECT * FROM messages 
        //              WHERE sender_id = @userId OR receiver_id = @userId 
        //              ORDER BY timestamp";
        //    return await connection.QueryAsync<Message>(query, new { userId });
        //}

        //public async Task<Message> SendMessage(Message message)
        //{
        //    using var connection = new NpgsqlConnection(_connectionString);
        //    var query = @"INSERT INTO messages(sender_id, receiver_id, content) 
        //              VALUES (@SenderId, @ReceiverId, @Content) 
        //              RETURNING *";
        //    return await connection.QuerySingleAsync<Message>(query, message);
        //}

        private readonly IDbConnection _db;

        public MessageRepository(IDbConnection db)
        {
            _db = db;
        }

        //public async Task<int> SaveMessageAsync(Message message)
        //{
        //    var query = @"INSERT INTO messages 
        //              (sender_id, receiver_id, content, timestamp, seen)
        //              VALUES (@SenderId, @ReceiverId, @Content, @Timestamp, @Seen)
        //              RETURNING id;";

        //    return await _db.ExecuteScalarAsync<int>(query, message);
        //}
        public async Task<int> SaveMessageAsync(Message message)
        {
            // Validate that sender and receiver exist to avoid FK violation.
            var senderCount = await _db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM \"Users\" WHERE id = @Id",
                new { Id = message.SenderId });

            if (senderCount == 0)
                throw new InvalidOperationException($"Sender with id {message.SenderId} does not exist.");

            var receiverCount = await _db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM \"Users\" WHERE id = @Id",
                new { Id = message.ReceiverId });

            if (receiverCount == 0)
                throw new InvalidOperationException($"Receiver with id {message.ReceiverId} does not exist.");

            var query = @"INSERT INTO messages 
                      (sender_id, receiver_id, content, timestamp, seen)
                      VALUES (@SenderId, @ReceiverId, @Content, @Timestamp, @Seen)
                      RETURNING id;";

            try
            {
                return await _db.ExecuteScalarAsync<int>(query, message);
            }
            catch (PostgresException ex)
            {
                // Provide a clearer error while preserving original exception for diagnostics.
                throw new InvalidOperationException($"Database error saving message: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(int user1, int user2)
        {
            var query = @"SELECT * FROM messages 
                      WHERE (sender_id = @user1 AND receiver_id = @user2)
                         OR (sender_id = @user2 AND receiver_id = @user1)
                      ORDER BY timestamp";

            return await _db.QueryAsync<Message>(query, new { user1, user2 });
        }

        public async Task MarkAsSeenAsync(int senderId, int receiverId)
        {
            var query = @"UPDATE messages 
                      SET seen = TRUE 
                      WHERE sender_id = @senderId AND receiver_id = @receiverId";

            await _db.ExecuteAsync(query, new { senderId, receiverId });
        }
    }
}

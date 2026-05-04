using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
using Npgsql;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class MessageRepository:IMessageRepository
    {
        private readonly IDbConnection _db;

        public MessageRepository(IDbConnection db)
        {
            _db = db;
        }
        public async Task<int> SaveMessageAsync(Message message)
        {
            // Validate that sender and receiver exist to avoid FK violation.
            var senderCount = await _db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM \"Users\" WHERE id = @Id", new { Id = message.Sender_Id });

            if (senderCount == 0)
                throw new InvalidOperationException($"Sender with id {message.Sender_Id} does not exist.");

            var receiverCount = await _db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM \"Users\" WHERE id = @Id", new { Id = message.Receiver_Id });

            if (receiverCount == 0)
                throw new InvalidOperationException($"Receiver with id {message.Receiver_Id} does not exist.");

            var query = @"INSERT INTO messages (sender_id, receiver_id, content, timestamp, seen) VALUES (@Sender_Id, @Receiver_Id, @Content, @Timestamp, @Seen) RETURNING id;";

            try
            {
                return await _db.ExecuteScalarAsync<int>(query, message);
            }
            catch (PostgresException ex)
            {
                throw new InvalidOperationException($"Database error saving message: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(int user1, int user2)
        {
            var query = @"SELECT * FROM messages WHERE (sender_id = @user1 AND receiver_id = @user2) OR (sender_id = @user2 AND receiver_id = @user1) ORDER BY timestamp";

            return await _db.QueryAsync<Message>(query, new { user1, user2 });
        }

        public async Task<List<int>> MarkAsSeenAsync(int senderId, int receiverId)
        {
            var query = @"UPDATE messages SET seen = TRUE WHERE sender_id = @receiverId AND receiver_id = @senderId AND seen = FALSE RETURNING ID";
            var seenMessageIds = await _db.QueryAsync<int>(query, new { senderId, receiverId });

            return seenMessageIds.ToList();
            //await _db.ExecuteAsync(query, new { senderId, receiverId });
            //var rows = await _db.ExecuteAsync(query, new { senderId, receiverId });
            //Console.WriteLine($"Rows updated: {rows}");
        }
        public int Count()
        {
            var query = "SELECT COUNT(*) FROM messages";
            return _db.ExecuteScalar<int>(query);
        }
    }
}

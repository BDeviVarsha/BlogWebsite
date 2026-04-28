using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
using Npgsql;

namespace BlogWebsite.Repositories.Implementation
{
    public class ModerationRepository:IModerationRepository
    {
        private readonly string _connectionString;

        public ModerationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Post>> GetPendingAnonymousPostsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            string sql = "SELECT * FROM Post WHERE name = 'Anonymous' AND status = 'Pending'";
            return await connection.QueryAsync<Post>(sql);
        }

        public async Task<bool> UpdatePostStatusAsync(int postId, string status)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            string sql = "UPDATE Post SET status = @Status WHERE postId = @PostId";
            var rows = await connection.ExecuteAsync(sql, new { Status = status, PostId = postId });
            return rows > 0;
        }
    }
}

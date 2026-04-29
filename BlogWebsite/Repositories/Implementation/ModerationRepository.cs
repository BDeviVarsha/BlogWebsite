using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
using Npgsql;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class ModerationRepository:IModerationRepository
    {
        private readonly IDbConnection _db;

        public ModerationRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Post>> GetPendingAnonymousPostsAsync()
        {
            string sql = "SELECT * FROM Post WHERE name = 'Anonymous' AND status = 'Pending'";
            return await _db.QueryAsync<Post>(sql);
        }

        public async Task<bool> UpdatePostStatusAsync(int postId, string status)
        {
            string sql = "UPDATE Post SET status = @Status WHERE postId = @PostId";
            var rows = await _db.ExecuteAsync(sql, new { Status = status, PostId = postId });
            return rows > 0;
        }
    }
}

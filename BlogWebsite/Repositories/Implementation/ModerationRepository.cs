using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
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
        public async Task<bool> ApprovePostAsync(int postId)
        {
            string sql = @"UPDATE Post 
                   SET status = 'Approved' WHERE postId = @PostId";

            var rows = await _db.ExecuteAsync(sql, new { PostId = postId});
            return rows > 0;
        }
        public async Task<bool> RejectPostAsync(int postId, string reason)
        {
            string sql = @"UPDATE Post 
                   SET status = 'Rejected', 
                       ""RejectedReason"" = @Reason 
                   WHERE postId = @PostId";

            var rows = await _db.ExecuteAsync(sql, new { PostId = postId, Reason = reason });
            return rows > 0;
        }
    }
}

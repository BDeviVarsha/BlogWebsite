using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        private readonly IDbConnection _db;

        public HomeRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Post>> GetAllPosts(int page, int limit)
        {
            int offset = (page - 1) * limit;
            var sql = @"SELECT * FROM post where status='Approved' ORDER BY postid DESC OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            return await _db.QueryAsync<Post>(sql, new { Offset = offset, Limit = limit });
        }

        public async Task<int> GetTotalPostsAsync()
        {
            var sql = "SELECT COUNT(*) FROM post";
            return await _db.ExecuteScalarAsync<int>(sql);
        }
        public async Task<IEnumerable<Post>> GetPostsByUserId(int userId)
        {
            var query = @"SELECT * FROM post WHERE UserId = @UserId ORDER BY postid DESC";

            return await _db.QueryAsync<Post>(query, new { UserId = userId });
        }
        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            var query = @"DELETE FROM post WHERE postid = @PostId AND UserId = @UserId";
            var result = await _db.ExecuteAsync(query, new { PostId = postId, UserId = userId });
            return result > 0;
        }
        public async Task<Post?> EditPostById(int postId, Post post)
        {
            var query = @"UPDATE post SET Title = @Title, Description = @Description, FilePath = @FilePath, Status = @Status, RejectedReason = @RejectedReason
                          WHERE postid = @PostId AND UserId = @UserId RETURNING *";
            return await _db.QueryFirstOrDefaultAsync<Post>(query, new
            {
                PostId = postId,
                post.Title,
                post.Description,
                post.FilePath,
                post.Status,
                post.RejectedReason,                
                post.UserId
            });
        }
        public async Task<IEnumerable<Announcement>> GetAnnouncement()
        {
            var query = @"SELECT * FROM Announcement order by id desc";
            return await _db.QueryAsync<Announcement>(query);
        }
    }
}

using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using System.Data;
using Dapper;

namespace BlogWebsite.Repositories.Implementation
{
    public class AdminRepository:IAdminRepository
    {
        private readonly IDbConnection _db;

        public AdminRepository(IDbConnection db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            var query = @"SELECT postid, Name, Title, Description, FilePath, Status FROM post where status != 'Rejected' ORDER BY postid DESC";
            return await _db.QueryAsync<Post>(query);
        }
        public async Task<bool> DeletePost(int id, string reason)
        {
            var query = @"UPDATE post SET status='Rejected', is_deleted = true, deleted_reason = @Reason WHERE postid = @Id";
            var result = await _db.ExecuteAsync(query, new { Id = id, Reason = reason });
            return result > 0;
        }

        public async Task<bool> RejectPost(int id, string reason)
        {
            var query = @"UPDATE post SET Status = 'Rejected', ""RejectedReason"" = @Reason WHERE postid = @Id";
            var result = await _db.ExecuteAsync(query, new { Id = id, Reason = reason });
            return result > 0;
        }

        public async Task<bool> ApprovePost(int id)
        {
            var query = @"UPDATE post SET Status = 'Approved' WHERE postid = @Id";
            var result = await _db.ExecuteAsync(query, new { Id = id });
            return result > 0;
        }
    }
}

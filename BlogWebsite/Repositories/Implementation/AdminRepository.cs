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
        public async Task<int> CreateAnnouncement(Announcement announcement)
        {
            var query = @"INSERT INTO Announcement (Title, Description, Is_Pinned, Created_at, Created_by) VALUES (@Title, @Description, @Is_Pinned, @Created_at, @Created_by)";
            return await _db.ExecuteAsync(query, announcement);
        }
        public async Task<IEnumerable<Announcement>> GetMyAnnouncements(string created_by)
        {
            var query = @"SELECT * FROM Announcement where created_by = @Created_by order by id desc";
            return await _db.QueryAsync<Announcement>(query, new { Created_by = created_by });
        }
        public async Task<bool> DeleteAnnouncementAsync(int id, string created_by)
        {
            var query = @"DELETE FROM Announcement where id =@Id AND created_by = @Created_by";
            var result = await _db.ExecuteAsync(query, new { Id = id, Created_by = created_by });
            return result > 0;
        }
        public async Task<Announcement> EditAnnouncementAsync(int id, Announcement announcement)
        {
            var query = @"UPDATE Announcement set title=@Title, description=@Description, is_pinned=@Is_Pinned where id=@Id and created_by = @Created_by returning *";
            return await _db.QueryFirstOrDefaultAsync<Announcement>(query, new { Id = id, announcement.Title, announcement.Description, announcement.Is_Pinned, Created_by = announcement.Created_by });
        }

    }
}

using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Post>> GetAllPosts();
        Task<bool> DeletePost(int id, string reason);
        Task<bool> RejectPost(int id, string reason);
        Task<bool> ApprovePost(int id);
        Task<int> CreateAnnouncement(Announcement announcement);
        Task<IEnumerable<Announcement>> GetMyAnnouncements(string createdBy);
        Task<bool> DeleteAnnouncementAsync(int id, string createdBy);
        Task<Announcement?> EditAnnouncementAsync(int id, Announcement announcement);

    }
}

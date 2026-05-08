using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IHomeRepository
    {
        public Task<IEnumerable<Post>> GetAllPosts(int page, int limit);
        Task<int> GetTotalPostsAsync();
        Task<IEnumerable<Post>> GetPostsByUserId(int userId);
        Task<bool> DeletePostAsync(int postId, int userId);
        Task<Post?> EditPostById(int postId, Post post);
        Task<IEnumerable<Announcement>> GetAnnouncement();
    }
}

using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Post>> GetAllPosts();
        Task<bool> DeletePost(int id, string reason);
        Task<bool> RejectPost(int id, string reason);
        Task<bool> ApprovePost(int id);
    }
}

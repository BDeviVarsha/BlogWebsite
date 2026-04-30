using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Post>> GetAllPosts();
        Task<bool> DeletePost(int id);
        Task<bool> RejectPost(int id);
        Task<bool> ApprovePost(int id);
    }
}

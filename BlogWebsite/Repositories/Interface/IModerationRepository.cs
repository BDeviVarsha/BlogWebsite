using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IModerationRepository
    {
        Task<IEnumerable<Post>> GetPendingAnonymousPostsAsync();
        Task<bool> UpdatePostStatusAsync(int postId, string status);
    }
}

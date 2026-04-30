using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IModerationRepository
    {
        Task<IEnumerable<Post>> GetPendingAnonymousPostsAsync();
        Task<bool> ApprovePostAsync(int postId);
        Task<bool> RejectPostAsync(int postId, string reason);
    }
}

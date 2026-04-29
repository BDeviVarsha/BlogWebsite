using BlogWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Repositories.Interface
{
    public interface IHomeRepository
    {
        public Task<IEnumerable<Post>> GetAllPosts(int page, int limit);
        Task<int> GetTotalPostsAsync();
    }
}

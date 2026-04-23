using BlogWebsite.DTOs;
using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IAddPostRepository
    {
        public Task<int> Create(PostDto postDto);
    }
}

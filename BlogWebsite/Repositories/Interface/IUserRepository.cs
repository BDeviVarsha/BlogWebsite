using BlogWebsite.Models;

namespace BlogWebsite.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<int> Register(Users user);
        Task<Users?> Login(long phoneNo, string role);
    }
}

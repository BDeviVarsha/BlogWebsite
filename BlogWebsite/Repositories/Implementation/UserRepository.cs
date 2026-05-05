using BlogWebsite.DTOs;
using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class UserRepository:IUserRepository
    {
        private readonly IDbConnection _db;

        public UserRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Register(Users user)
        {
            var sql = "INSERT INTO \"Users\" (name, \"phoneNo\", role) VALUES (@Name, @PhoneNo, @Role);";
            var id = await _db.ExecuteAsync(sql, user);

            return id;
        }

        public async Task<Users?> Login(long phoneNo, string role)
        {
            var user = await _db.QuerySingleOrDefaultAsync<Users>(@"SELECT * FROM ""Users""  WHERE ""phoneNo"" = @PhoneNo AND role = @Role", new { PhoneNo = phoneNo, Role = role });

            if (user == null)
                return null;

            await _db.ExecuteAsync(@"UPDATE ""Users"" SET is_active = true, updated_at = NOW() WHERE id = @Id", new { user.Id });

            return new Users
            {
                Id = user.Id,
                Name = user.Name,
                phoneNo = user.phoneNo,
                Role = user.Role,
                is_active = true
            };
        }
        public async Task<IEnumerable<Roles>> GetRoles()
        {
            var sql = "SELECT id, name FROM roles ORDER BY name";
            return await _db.QueryAsync<Roles>(sql);
        }
        public async Task<bool> Logout(int userId)
        {
            var sql = @"UPDATE ""Users"" SET is_active = false, updated_at = NOW() WHERE id = @Id";
            var rows = await _db.ExecuteAsync(sql, new { Id = userId });
            return rows > 0;
        }
        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {

            var sql = @"SELECT id, name, is_active FROM ""Users"" ORDER BY id;";

            return await _db.QueryAsync<Users>(sql);
        }
    }
}

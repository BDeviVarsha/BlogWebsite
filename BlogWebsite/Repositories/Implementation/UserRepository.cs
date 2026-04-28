using BlogWebsite.DTOs;
using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using Dapper;
using Npgsql;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class UserRepository:IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Register(Users user)
        {
            using var connection = new Npgsql.NpgsqlConnection(_connectionString);

            var sql = "INSERT INTO \"Users\" (name, \"phoneNo\", role) VALUES (@Name, @PhoneNo, @Role);";
            var id = await connection.ExecuteAsync(sql, user);

            return id;
        }

        public async Task<Users?> Login(long phoneNo, string role)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var user = await connection.QuerySingleOrDefaultAsync<Users>(@"SELECT * FROM ""Users""  WHERE ""phoneNo"" = @PhoneNo AND role = @Role", new { PhoneNo = phoneNo, Role = role });

            if (user == null)
                return null;

            await connection.ExecuteAsync(@"UPDATE ""Users"" SET is_active = true, updated_at = NOW() WHERE id = @Id", new { user.Id });

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
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = "SELECT id, name FROM roles ORDER BY name";
            return await connection.QueryAsync<Roles>(sql);
        }
        public async Task<bool> Logout(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var rows = await connection.ExecuteAsync(@"UPDATE ""Users"" SET is_active = false, updated_at = NOW() WHERE id = @Id",  new { Id = userId });
            return rows > 0;
        }
        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var sql = @"SELECT id, name, is_active FROM ""Users"" ORDER BY id;";

            return await connection.QueryAsync<Users>(sql);
        }
    }
}

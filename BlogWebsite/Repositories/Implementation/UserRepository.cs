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

            var sql = "INSERT INTO users (name, \"phoneNo\", role) VALUES (@Name, @PhoneNo, @Role);";
            var id = await connection.ExecuteAsync(sql, user);

            return id;
        }

        public async Task<Users?> Login(long phoneNo, string role)
        {
            using var connection = new Npgsql.NpgsqlConnection(_connectionString);

            var sql = "SELECT * FROM users WHERE \"phoneNo\" = @PhoneNo AND role = @Role";
            return await connection.QuerySingleOrDefaultAsync<Users>(sql, new { phoneNo = phoneNo, role = role });
        }
        public async Task<IEnumerable<Roles>> GetRoles()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = "SELECT id, name FROM roles ORDER BY name";
            return await connection.QueryAsync<Roles>(sql);
        }
    }
}

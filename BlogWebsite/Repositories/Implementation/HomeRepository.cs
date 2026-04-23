using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;

namespace BlogWebsite.Repositories.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        public SftpService _sftpService;
        private readonly string _connectionString;

        public HomeRepository(IConfiguration configuration, SftpService sftpService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _sftpService = sftpService;
        }
        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            using var connection = new Npgsql.NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM post";
            return await connection.QueryAsync<Post>(query);
        }
    }
}

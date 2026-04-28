using BlogWebsite.DTOs;
using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Dapper;
using Npgsql;
using Org.BouncyCastle.Asn1.X509;
using Renci.SshNet;

namespace BlogWebsite.Repositories.Implementation
{
    public class AddPostRepository : IAddPostRepository
    {
        public SftpService _sftpService;
        private readonly string _connectionString;

        public AddPostRepository(IConfiguration configuration, SftpService sftpService)
        {
            _sftpService = sftpService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Create(PostDto postDto)
        {
            string remoteFilePath = null;

            if (postDto.File != null && postDto.File.Length > 0)
            {
                using var ms = new MemoryStream();
                await postDto.File.CopyToAsync(ms);
                _sftpService.UploadFile(ms, postDto.File.FileName);
                remoteFilePath = $"{_sftpService.BasePath}/{postDto.File.FileName}";
            }
            // Determine status: anonymous submissions are pending
            var isAnonymous = string.IsNullOrWhiteSpace(postDto.Name)
                              || string.Equals(postDto.Name, "Anonymous", StringComparison.OrdinalIgnoreCase);
            var status = isAnonymous ? "Pending" : "Approved";

            using var connection = new NpgsqlConnection(_connectionString);
            string sql = @"INSERT INTO Post (name, title, description, filepath, status) VALUES (@Name, @Title, @Description, @FilePath, @Status)";
            return await connection.ExecuteAsync(sql, new
            {
                postDto.Name,
                postDto.Title,
                postDto.Description,
                FilePath = remoteFilePath,
                Status = status
            });
        }
    }
}
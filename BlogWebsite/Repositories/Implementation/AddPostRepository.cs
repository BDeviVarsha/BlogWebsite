using BlogWebsite.DTOs;
using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Dapper;
using Npgsql;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class AddPostRepository : IAddPostRepository
    {
        public SftpService _sftpService;
        private readonly IDbConnection _db;

        public AddPostRepository(IDbConnection db, SftpService sftpService)
        {
            _db = db;
            _sftpService = sftpService;
        }

        public async Task<int> Create(PostDto postDto)
        {
            List<string> remoteFilePaths = new();

            if (postDto.Files != null && postDto.Files.Any())
            {
                foreach (var file in postDto.Files)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        _sftpService.UploadFile(ms, file.FileName);
                        remoteFilePaths.Add($"{_sftpService.BasePath}/{file.FileName}");
                    }
                }
            }

            var filesCsv = string.Join(",", remoteFilePaths); // store as CSV or create a separate table

            var isAnonymous = string.IsNullOrWhiteSpace(postDto.Name) || string.Equals(postDto.Name, "Anonymous", StringComparison.OrdinalIgnoreCase);
            var status = isAnonymous ? "Pending" : "Approved";

            string sql = @"INSERT INTO Post (name, title, description, filepath, status) VALUES (@Name, @Title, @Description, @FilePath, @Status)";
            return await _db.ExecuteAsync(sql, new
            {
                postDto.Name,
                postDto.Title,
                postDto.Description,
                FilePath = filesCsv,
                Status = status
            });
        }
    }
}
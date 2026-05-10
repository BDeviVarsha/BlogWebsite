using BlogWebsite.DTOs;
using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Dapper;
using System.Data;
using System.Net.NetworkInformation;

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
                        var remoteFilePath = await _sftpService.UploadFileAsync(file);
                        remoteFilePaths.Add(remoteFilePath);
                    }
                }
            }

            var filesCsv = string.Join(",", remoteFilePaths); // store as CSV or create a separate table

            var isAnonymous = string.IsNullOrWhiteSpace(postDto.Name) || string.Equals(postDto.Name, "Anonymous", StringComparison.OrdinalIgnoreCase);
            var status = isAnonymous ? "Pending" : "Approved";

            string sql = @"INSERT INTO Post (name, title, description, filepath, status, userid) VALUES (@Name, @Title, @Description, @FilePath, @Status, @UserId)";
            return await _db.ExecuteAsync(sql, new
            {
                postDto.Name,
                postDto.Title,
                postDto.Description,
                FilePath = filesCsv,
                Status = status,
                postDto.UserId,

            });
        }
        public async Task<Post?> Update(int postId, PostDto postDto)
        {
            List<string> remoteFilePaths = new();

            if (postDto.Files != null && postDto.Files.Any())
            {
                foreach (var file in postDto.Files)
                {
                    if (file.Length > 0)
                    {
                        var remoteFilePath = await _sftpService.UploadFileAsync(file);
                        remoteFilePaths.Add(remoteFilePath);
                    }
                }
            }

            // If no new files uploaded, check for existing file path sent from frontend
            string filesCsv;
            if (remoteFilePaths.Any())
            {
                filesCsv = string.Join(",", remoteFilePaths);
            }
            else
            {
                // Fallback: keep existing filepath (frontend sends it as 'existingFilePath' form field)
                // We'll re-query the existing post to preserve its filepath
                var existing = await _db.QueryFirstOrDefaultAsync<Post>(
                    "SELECT * FROM post WHERE postid = @PostId AND userid = @UserId",
                    new { PostId = postId, UserId = postDto.UserId });

                filesCsv = existing?.FilePath ?? "";
            }

            var sql = @"UPDATE post 
                SET title = @Title, description = @Description, filepath = @FilePath
                WHERE postid = @PostId AND userid = @UserId
                RETURNING *";

            return await _db.QueryFirstOrDefaultAsync<Post>(sql, new
            {
                PostId = postId,
                postDto.Title,
                postDto.Description,
                FilePath = filesCsv,
                UserId = postDto.UserId
            });
        }
    }
}
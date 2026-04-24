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
            //string folder = "uploads";
            //var fileName = $"{Guid.NewGuid()}_{postDto.File.FileName}";
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder, fileName);

            //// Ensure directory exists
            //Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            //// Save post info to database
            //var post = new Post
            //{
            //    Name = postDto.Name,
            //    Title = postDto.Title,
            //    Description = postDto.Description,
            //    FilePath = $"/{folder}/{fileName}", // accessible via HTTP
            //    //MediaType = file.ContentType.Split('/')[0] // "image", "video", or "audio"
            //};

            //// Save `post` to your database here (e.g., via Dapper or EF Core)
            //var postId = await _postRepository.Create(post);

            //return Ok(new { message = "Post created", postId, filePath = post.FilePath });

            if (postDto.File != null && postDto.File.Length > 0)
            {
                using var ms = new MemoryStream();
                await postDto.File.CopyToAsync(ms);
                _sftpService.UploadFile(ms, postDto.File.FileName);
                remoteFilePath = $"{_sftpService.BasePath}/{postDto.File.FileName}";
            }

            using var connection = new NpgsqlConnection(_connectionString);
            string sql = @"INSERT INTO Post (name, title, description, filepath) VALUES (@Name, @Title, @Description, @FilePath)";

            return await connection.ExecuteAsync(sql, new
            {
                postDto.Name,
                postDto.Title,
                postDto.Description,
                FilePath = remoteFilePath
            });
        }
    }
}
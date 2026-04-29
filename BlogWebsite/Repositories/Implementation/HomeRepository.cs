using BlogWebsite.Models;
using BlogWebsite.Repositories.Interface;
using BlogWebsite.Services;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BlogWebsite.Repositories.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        private readonly IDbConnection _db;

        public HomeRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Post>> GetAllPosts(int page, int limit)
        {
            int offset = (page - 1) * limit;
            var sql = @"SELECT * FROM post OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            return await _db.QueryAsync<Post>(sql, new { Offset = offset, Limit = limit });
        }

        public async Task<int> GetTotalPostsAsync()
        {
            var sql = "SELECT COUNT(*) FROM post";
            return await _db.ExecuteScalarAsync<int>(sql);
        }
    }
}

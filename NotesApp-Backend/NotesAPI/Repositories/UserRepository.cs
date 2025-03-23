using Dapper;
using Microsoft.Data.SqlClient;
using NotesAPI.Models;

namespace NotesAPI.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<int> CreateUser(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
            return await connection.ExecuteAsync(sql, user);
        }
    }
}

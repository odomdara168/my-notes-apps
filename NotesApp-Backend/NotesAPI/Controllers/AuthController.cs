using Microsoft.AspNetCore.Mvc;
using NotesAPI.Data;
using NotesAPI.Models;
using NotesAPI.Services;
using Dapper;
using BCrypt.Net;
using System.Threading.Tasks;

namespace NotesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly TokenService _tokenService;

        public AuthController(DapperContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            using var connection = _context.CreateConnection();

            var userExists = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username",
                new { userDto.Username });

            if (userExists != null)
                return BadRequest(new { message = "User already exists" });

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var sql = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
            await connection.ExecuteAsync(sql, new { userDto.Username, PasswordHash = passwordHash });

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            using var connection = _context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username",
                new { userDto.Username });

            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _tokenService.CreateToken(user.Id, user.Username);

            return Ok(new { token });
        }
    }
}

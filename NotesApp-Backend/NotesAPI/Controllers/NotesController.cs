using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesAPI.Data;
using NotesAPI.Models;
using Dapper;
using System.Security.Claims;

namespace NotesAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly DapperContext _context;

        public NotesController(DapperContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet]
        public async Task<IActionResult> GetNotes(
            [FromQuery] string? title,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1)
        {
            var userId = GetUserId();
            using var connection = _context.CreateConnection();

            const int pageSize = 10;

            var validSortBy = new[] { "CreatedAt", "UpdatedAt", "Title" };
            var validSortOrder = new[] { "asc", "desc" };

            sortBy = !string.IsNullOrEmpty(sortBy) && validSortBy.Contains(sortBy)
                ? sortBy
                : "CreatedAt";

            sortOrder = !string.IsNullOrEmpty(sortOrder) && validSortOrder.Contains(sortOrder.ToLower())
                ? sortOrder.ToLower()
                : "desc";

            if (page < 1) page = 1;

            var offset = (page - 1) * pageSize;

            var whereClause = "WHERE UserId = @UserId";
            if (!string.IsNullOrEmpty(title))
            {
                whereClause += " AND Title LIKE @Title";
            }

            var countSql = $"SELECT COUNT(*) FROM Notes {whereClause}";

            var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, new
            {
                UserId = userId,
                Title = $"%{title}%"
            });

            var dataSql = $@"
                SELECT Id, Title, CreatedAt, UpdatedAt
                FROM Notes
                {whereClause}
                ORDER BY {sortBy} {sortOrder.ToUpper()}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var notes = await connection.QueryAsync<Note>(dataSql, new
            {
                UserId = userId,
                Title = $"%{title}%",
                Offset = offset,
                PageSize = pageSize
            });

            var response = new
            {
                data = notes,
                pagination = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalRecords = totalRecords,
                    totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
                }
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var userId = GetUserId();
            using var connection = _context.CreateConnection();

            var sql = "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId";

            var note = await connection.QuerySingleOrDefaultAsync<Note>(sql, new
            {
                Id = id,
                UserId = userId
            });

            if (note == null)
                return NotFound("Note not found");

            return Ok(note);
        }


        [HttpPost]
        public async Task<IActionResult> CreateNote(Note note)
        {
            if (string.IsNullOrWhiteSpace(note.Title))
                return BadRequest("Title is required.");

            var userId = GetUserId();
            using var connection = _context.CreateConnection();

            var sql = @"INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
                        VALUES (@UserId, @Title, @Content, GETDATE(), GETDATE())";

            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                note.Title,
                note.Content
            });

            return Ok("Note created");
        }

        /// <summary>
        /// Update an existing note
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, Note note)
        {
            if (string.IsNullOrWhiteSpace(note.Title))
                return BadRequest("Title is required.");

            var userId = GetUserId();
            using var connection = _context.CreateConnection();

            var sql = @"UPDATE Notes 
                        SET Title = @Title, Content = @Content, UpdatedAt = GETDATE()
                        WHERE Id = @Id AND UserId = @UserId";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                note.Title,
                note.Content,
                Id = id,
                UserId = userId
            });

            if (affectedRows == 0)
                return NotFound("Note not found");

            return Ok("Note updated");
        }

 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = GetUserId();
            using var connection = _context.CreateConnection();

            var sql = "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                UserId = userId
            });

            if (affectedRows == 0)
                return NotFound("Note not found");

            return Ok("Note deleted");
        }
    }
}

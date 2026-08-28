using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using System.Threading.Tasks;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromQuery] string email, [FromQuery] string role)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
        {
            return BadRequest(new { Error = "Email and role are required." });
        }

        var validRoles = new[] { "Admin", "Moderator", "User" };
        if (!System.Array.Exists(validRoles, r => r == role))
        {
            return BadRequest(new { Error = "Invalid role. Valid roles are Admin, Moderator, User." });
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return NotFound(new { Error = "User not found." });
        }

        user.Role = role;
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"Role {role} assigned to user {email} successfully." });
    }
}

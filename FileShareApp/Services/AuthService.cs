using FileShareApp.Backend.Helpers;
using FileShareApp.Backend.Services.Interfaces;
using FileShareApp.Backend.Data;
using FileShareApp.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace FileShareApp.Backend.Services;

public class AuthService(AppDbContext db, JwtHelper jwt) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly JwtHelper _jwt = jwt;

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (await _db.Users.AnyAsync(u => u.Email == email))
            return false;

        _db.Users.Add(new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        });

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return _jwt.GenerateToken(user.Email);
    }
}
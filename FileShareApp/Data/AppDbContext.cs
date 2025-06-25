using Microsoft.EntityFrameworkCore;
using FileShareApp.Backend.Models;

namespace FileShareApp.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated(); 
    }

    public DbSet<User> Users => Set<User>();
}
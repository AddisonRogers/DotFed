using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace DotFed;

using Microsoft.EntityFrameworkCore;

public class Db : DbContext
{
    public Db(DbContextOptions<Db> options)
        : base(options) { }
    
    public DbSet<User>? Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("test");
        
    }
    
    
    
}

public class User
{
    [Key]    
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required MailAddress Email { get; set; }
    public JsonDocument Data { get; set; }
    
    
    
}
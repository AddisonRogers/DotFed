using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace DotFed;

using Microsoft.EntityFrameworkCore;

public class Db : DbContext
{
    public Db(DbContextOptions<Db> options)
        : base(options)
    {
        
    }

    public DbSet<User>? Users { get; set; }
}

public class User
{
    [Key]    
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required MailAddress Email { get; set; }
    public JsonDocument Data { get; set; }
}

/*
DbSet<User> users = Users;
        users.Add(new User
        {
            Username = "test",
            Password = "test",
            Email = new MailAddress("   "),
        });
        SaveChanges();
        */
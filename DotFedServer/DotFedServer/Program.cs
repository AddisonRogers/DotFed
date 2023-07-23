using System.Net.Mail;
using System.Text.Json;
using DotFedServer;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.Results;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<Db>(opt => opt.UseInMemoryDatabase("test"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
using var db = new Db(new DbContextOptionsBuilder<Db>().UseInMemoryDatabase("test").Options);

app.MapGet("/", string () => "Hello World!");

//app.MapGet("/users", List<User>? () => db.Users?.ToList());

app.MapPost("/user/add", async Task<IResult> (string username, string password, string email) =>
{
    if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email)) return BadRequest();
    if (MailAddress.TryCreate(email, out var mailAddress)) return BadRequest("Invalid email address");
    if(db == null) return BadRequest("Database is null");
    if(await db.Users.AnyAsync(u => u.Username == username)) return BadRequest("User already exists");
    
    var hashedPw = Argon2.Hash(password);
    
    var user = new User
    {
        Username = username,
        Password = hashedPw,
        Email = mailAddress!
    }; 
    
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Ok(200);
});

app.MapGet("/user/{id}", User? (string id) => db.Users?.FirstOrDefault(u => u.Username == id));
app.MapDelete("/user/{id}", async Task<IResult> (string id) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    db.Users?.Remove(user);
    await db.SaveChangesAsync();
    return Ok(200);
});
app.MapPut("/user/{id}", async Task<IResult> (string id, string? newId, string? password, string? email) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    if (!string.IsNullOrEmpty(newId)) user.Username = newId;
    if (!string.IsNullOrEmpty(password)) user.Password = Argon2.Hash(password);
    if (!string.IsNullOrEmpty(email)) user.Email = new MailAddress(email);
    await db.SaveChangesAsync();
    return Ok(200);
});

app.MapGet("/user/{id}/password/{password}", async Task<IResult> (string id, string password) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    if (Argon2.Verify(user.Password, password)) return Ok(200);
    return BadRequest("Invalid password");
});
app.MapGet("/user/{id}/email/{email}", async Task<IResult> (string id, string email) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    if (user.Email.Address == email) return Ok(200);
    return BadRequest("Invalid email");
});

app.MapGet("/user/{id}/data", async Task<IResult> (string id) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    return Ok(user.Data);
});
app.MapPut("/user/{id}/data", async Task<IResult> (string id, string rawData) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    var data = JsonSerializer.Deserialize<JsonDocument>(rawData);
    if (data == null) return BadRequest("Invalid data");
    user.Data = data;
    await db.SaveChangesAsync();
    return Ok(200);
});
app.MapPost("/user/{id}/data", async Task<IResult> (string id, string rawData) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    var data = JsonSerializer.Deserialize<JsonDocument>(rawData);
    if (data == null) return BadRequest("Invalid data");
    user.Data = data;
    await db.SaveChangesAsync();
    return Ok(200);
});
app.MapDelete("/user/{id}/data", async Task<IResult> (string id) =>
{
    var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
    if (user == null) return NotFound();
    user.Data = null;
    await db.SaveChangesAsync();
    return Ok(200);
});

app.Run();
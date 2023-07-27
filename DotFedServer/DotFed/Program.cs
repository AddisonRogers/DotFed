using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DotFedServer;
using HtmlAgilityPack;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.AspNetCore.Http.Results;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
using var db = new Db(new DbContextOptionsBuilder<Db>().UseInMemoryDatabase("test").Options);

app.MapGet("/", string () => "Hello World!");

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
    
    // TODO add return JWT
    
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

app.MapGet("/list", async Task<JsonElement> () =>
{
    /*
    var client = new HttpClient();
    var request = new HttpRequestMessage(HttpMethod.Post, "https://the-federation.info/v1/graphql");
    var content = new StringContent("{\"query\":\"query MyQuery {\\r\\n                thefederation_node(where: { thefederation_platform: { name: { _iregex: \\\"(lemmy|kbin)\\\" } } }) {\\r\\n                name\\r\\n                open_signups\\r\\n                thefederation_platform {\\r\\n                name\\r\\n                }\\r\\n                }\\r\\n                }\",\"variables\":{}}", null, "application/json");
    request.Content = content;
    var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadAsStringAsync();
    var json = JsonDocument.Parse(result);
    var nodes = json.RootElement.GetProperty("data").GetProperty("thefederation_node");
    */
    
    var json = JsonDocument.Parse(await System.IO.File.ReadAllTextAsync("data.json"));
    return json.RootElement.GetProperty("data").GetProperty("thefederation_node");
});

app.MapGet("/list/{server}", async Task<JsonArray> (string server) =>
    {
        var json = JsonDocument.Parse(await System.IO.File.ReadAllTextAsync("data.json"));
        var nodes = json.RootElement.GetProperty("data").GetProperty("thefederation_node");
        var specialisedNodes = new JsonArray();
        foreach (var node in nodes.EnumerateArray())
        {
            if (node.GetProperty("thefederation_platform").GetProperty("name").GetString() == server)
            {
                specialisedNodes.Add(node.GetProperty("name"));
            }
        }
        
        return specialisedNodes;
    });




/*
 * {
        "name": "lemmy.nauk.io",
        "open_signups": false,
        "thefederation_platform": {
          "name": "lemmy"
        }
      },
 */
app.Run();

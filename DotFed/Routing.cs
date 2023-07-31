using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Nodes;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.Results;

namespace DotFed;

public static class Routing
{
    static readonly HttpClient client = new HttpClient();
    
    
    public static void AddRoutes(this WebApplication app, Db? db)
    {
        
        
        
        app.MapGet("/", () => Results.Extensions.RazorSlice("/Slices/Index.cshtml", "NAME"));
        app.MapPost("/", () => Content("<span>Hello World</span>", "text/html"));
        //https://htmx.org/examples/infinite-scroll/
        
        
        AddMain(app.MapGroup("/"), db);
        AddAPI(app.MapGroup("/API"), db);
    }

    private static void AddMain(RouteGroupBuilder app, Db? db)
    {
        
    }
    
    
    
    
    private static void AddAPI(RouteGroupBuilder app, Db? db)
    {
        app.MapGet("/Lemmy",
            () => (Lemmy.GetData(Lemmy.NewConnection(client,"lemmy.ml", null), null, null, null)));
        
        
        
        
        var user = app.MapGroup("/user");
        
        user.MapPost("/user/add", async Task<IResult> (string username, string password, string email) =>
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email)) return BadRequest();
            if (MailAddress.TryCreate(email, out var mailAddress)) return BadRequest("Invalid email address");
            if(db == null) return BadRequest("Database is null");
            if(db.Users != null && await db.Users.AnyAsync(u => u.Username == username)) return BadRequest("User already exists");
            
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
        
        var id = user.MapGroup("/{id}");
        
        {
            id.MapGet("/", User? (string id) => db.Users?.FirstOrDefault(u => u.Username == id));
            id.MapDelete("/", async Task<IResult> (string id) =>
            {
                var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id)!;
                if (user == null) return NotFound();
                db.Users?.Remove(user);
                await db.SaveChangesAsync();
                return Ok(200);
            });
            id.MapPut("/", async Task<IResult> (string id, string? newId, string? password, string? email) =>
            {
                var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
                if (user == null) return NotFound();
                if (!string.IsNullOrEmpty(newId)) user.Username = newId;
                if (!string.IsNullOrEmpty(password)) user.Password = Argon2.Hash(password);
                if (!string.IsNullOrEmpty(email)) user.Email = new MailAddress(email);
                await db.SaveChangesAsync();
                return Ok(200);
            });
        }

        id.MapGet("/password/{password}", async Task<IResult> (string id, string password) =>
        {
            var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
            if (user == null) return NotFound();
            if (Argon2.Verify(user.Password, password)) return Ok(200);
            return BadRequest("Invalid password");
        });
        id.MapGet("/email/{email}", async Task<IResult> (string id, string email) =>
        {
            var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
            if (user == null) return NotFound();
            if (user.Email.Address == email) return Ok(200);
            return BadRequest("Invalid email");
        });

        var data = id.MapGroup("/data");

        {
            data.MapGet("/", async Task<IResult> (string id) =>
            {
                var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
                if (user == null) return NotFound();
                return Ok(user.Data);
            });
            data.MapPut("/", async Task<IResult> (string id, string rawData) =>
            {
                var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
                if (user == null) return NotFound();
                var data = JsonSerializer.Deserialize<JsonDocument>(rawData);
                if (data == null) return BadRequest("Invalid data");
                user.Data = data;
                await db.SaveChangesAsync();
                return Ok(200);
            });
            data.MapPost("/", async Task<IResult> (string id, string rawData) =>
            {
                var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
                if (user == null) return NotFound();
                var data = JsonSerializer.Deserialize<JsonDocument>(rawData);
                if (data == null) return BadRequest("Invalid data");
                user.Data = data;
                await db.SaveChangesAsync();
                return Ok(200);
            });
            data.MapDelete("/", async Task<IResult> (string id) =>
            {
                var user = await db.Users?.FirstOrDefaultAsync(u => u.Username == id);
                if (user == null) return NotFound();
                user.Data = null;
                await db.SaveChangesAsync();
                return Ok(200);
            });
        }

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
        }
    
    
    
    
    
    
    
    
}
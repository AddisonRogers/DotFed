using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Nodes;
using DotFed.Slices;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.Results;

namespace DotFed;

public static class Routing
{
    static readonly HttpClient client = new HttpClient();
    
    public static void AddRoutes(this WebApplication app, Db? db)
    {
        IndexModel.Routes(app.MapGroup("/"), db);
        
        
        
        
        
        /*
        app.MapPost("/", () => Content("<span>Hello World</span>", "text/html"));
        //https://htmx.org/examples/infinite-scroll/
        */
        
    }
}
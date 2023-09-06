using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Nodes;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.Results;

namespace DotFed;

public static class Routing
<<<<<<< HEAD
{    
    public static void AddRoutes(this WebApplication app)
    {
        
=======
{
    static readonly HttpClient client = new HttpClient();
    
    public static void AddRoutes(this WebApplication app)
    {
        IndexModel.Routes(app.MapGroup("/"));
>>>>>>> 170192b3902b2a72fe7a53763c367d0760cdbf70
        
        
        
        
        
        /*
        app.MapPost("/", () => Content("<span>Hello World</span>", "text/html"));
        //https://htmx.org/examples/infinite-scroll/
        */
        
    }
}
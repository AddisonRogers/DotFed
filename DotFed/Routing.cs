using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Nodes;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.Results;

namespace DotFed;

public static class Routing
{
    
    static int i = 0;
    public static void AddRoutes(this WebApplication app)
    {
        
        app.Map("/", () => Misc.NewPage(Misc.HelloBlank(i.ToString())));
        




        /*
        app.MapPost("/", () => Content("<span>Hello World</span>", "text/html"));
        //https://htmx.org/examples/infinite-scroll/
        */

    }
}
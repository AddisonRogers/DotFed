using Htmx;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotFed.Slices;

public class IndexModel
{
    public static int count = 0;
    public static void Routes(RouteGroupBuilder app)
    {
        app.MapGet("/", () => Results.Extensions.RazorSlice("/Slices/Index.cshtml"));
        app.MapPost("/", (int? page, HttpRequest request) => request.IsHtmx()
            ? Results.Extensions.RazorSlice("/Slices/_post.cshtml", page)
            : Results.Extensions.RazorSlice("/Slices/Index.cshtml"));
    }
    
    
    
}




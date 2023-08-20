using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotFed.Slices;

public class IndexModel
{
    public static void Routes(RouteGroupBuilder app)
    {
        app.MapGet("/", () => Results.Extensions.RazorSlice("/Slices/Index.cshtml"));
        
    }
    
    
    
}




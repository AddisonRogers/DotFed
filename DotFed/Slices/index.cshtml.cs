using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotFed.Slices;

public class IndexModel
{
    public static string Title = "DotFed";
    public static int UpvotesPercent = 50;
    public static int DownvotesPercent = 50;
    public static string GradientStyle = $"background: linear-gradient(to bottom, rgba(0,255,0,1) 0%,rgba(0,255,0,0.5) {UpvotesPercent - 10}%, rgba(255,0,0,0.5) {DownvotesPercent - 10}%,rgba(255,0,0,1) 100%);";
    
    public static void Routes(RouteGroupBuilder app)
    {
        app.MapGet("/", () => Results.Extensions.RazorSlice("/Slices/Index.cshtml"));
    }
    
    
    
}




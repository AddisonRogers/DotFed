namespace DotFed.Slices;

public static class IndexModel
{
    public static void Routes(RouteGroupBuilder app, Db? db)
    {
        app.MapGet("/", () => Results.Extensions.RazorSlice("/Slices/Index.cshtml"));
    }
    
    public static List<string> Posts { get; set; } = new List<string>(){"Hello World"};
    
    
}




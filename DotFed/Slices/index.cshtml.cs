using System.Text;
using Htmx;
using Microsoft.Extensions.Primitives;
using static Microsoft.AspNetCore.Http.Results;

namespace DotFed.Slices;

public class IndexModel
{
    public static int count = 0;
    public static void Routes(RouteGroupBuilder app)
    {
        app.MapGet("/", () => Results.Extensions.RazorSlice("/Slices/Index.cshtml"));
        app.MapPost("/", (int? page, HttpRequest request) => request.IsHtmx()
            ? MorePosts(page ?? 1)
            : Results.Extensions.RazorSlice("/Slices/Index.cshtml"));
    }

    private static IResult MorePosts(int page)
    {
        var html = new StringBuilder();
        html.Append("<div hx-get='/'>Page: " + page + "</div>");
        for (var i = 1; i <= 10; i++)
        {
            if (i == 10)
            {
                count++;
                html.Append(
                    $"""<div hx-post="/?page={count}" hx-trigger="revealed" hx-swap="afterend">Post {i}</div>""");

            }
            else
            {
                html.Append($"<div>Post {i}</div>");
            }
        }

        return Content(html.ToString(), "text/html");
    }
    
}




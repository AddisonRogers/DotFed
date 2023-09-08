using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.HttpResults;

public static class Misc
{
    public static string HelloWorld() {

        //System.Console.WriteLine("ooh wee I can do code and get props");


        return ("<h1>Hello World mf</h1>");
    }

    public static string HelloBlank(string props) {

        System.Console.WriteLine($"{props}");


        return ($"<h1>Hello {props} mf</h1>");
    }
    
    

    public static ContentHttpResult NewPage(string body) {

        return TypedResults.Content(
            $"""
            <!DOCTYPE html>
            <html>
                <head>

                    <title>DotFed</title>
                    <script src="https://unpkg.com/htmx.org@1.9.4" integrity="sha384-zUfuhFKKZCbHTY6aRR46gxiqszMk5tcHjsVFxnUo8VMus4kHGVdIYVbOYYNlKmHV" crossorigin="anonymous"></script>
                    <script src="https://unpkg.com/hyperscript.org@0.9.11"></script>
                    <script src="https://cdn.jsdelivr.net/npm/@unocss/runtime/core.global.js"></script>        
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <meta charset="UTF-8">
                    

                </head>

                <body>
                {body}
                </body>
            </html>
            """, "text/html"
            );
    }

}


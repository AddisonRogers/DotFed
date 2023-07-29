using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotFed.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        
    }
    
    public IActionResult OnPost()
    {
        return Request.IsHtmx()
            ? Content("<span>Hello, World!</span>", "text/html")
            : Page();
    }
}
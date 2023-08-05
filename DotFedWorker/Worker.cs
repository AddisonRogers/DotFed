using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Playwright;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace DotFedWorker;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public Dictionary<string,string> urlPlatformMap = new Dictionary<string, string>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var razorBlade = new RazorBlade();
        razorBlade.GiveBirth();
        
        ReadJson();
        
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "hello",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        Console.WriteLine(" [*] Waiting for messages.");
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body); // message is a url to scrape
            Console.WriteLine($" [x] Received {message}");
            ValidateAndSwitch(message);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        
        channel.BasicConsume(queue: "hello",
            autoAck: false,
            consumer: consumer);
        
        
        while (!stoppingToken.IsCancellationRequested)
        {
            RazorBlade.Scrape("https://kbin.social/all", "Kbin");
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            Thread.Sleep(1000);
        }
    }

    async void ReadJson()
    {
        using var jsonDoc = JsonDocument.Parse(await File.ReadAllTextAsync("data.json"));

        var root = jsonDoc.RootElement;
        var dataArray = root.GetProperty("data").GetProperty("thefederation_node");
        
        foreach (var item in dataArray.EnumerateArray())
        {
            var name = item.GetProperty("name").GetString();
            var platform = item.GetProperty("thefederation_platform").GetProperty("name").GetString();
            urlPlatformMap[name] = platform;
        }
    }
    
    
    async void ValidateAndSwitch(string url)
    {
        var Valurl = url;
        if (!Valurl.StartsWith("http://") && !Valurl.StartsWith("https://")) Valurl = "https://" + url;
        if (Uri.TryCreate(Valurl, UriKind.Absolute, out var validatedUri)
            && (validatedUri.Scheme == Uri.UriSchemeHttps))
        {
            Console.WriteLine($" [x] Received valid URL: {url}");

            if (urlPlatformMap.TryGetValue(url, out var platform))
            {
                Console.WriteLine($" [x] {url} is a valid URL");
                Console.WriteLine($" [x] {url} is a {platform} instance");
                await RazorBlade.Scrape(url, platform);
            }
            else
            {
                Console.WriteLine($" [x] {url} is a valid URL");
                Console.WriteLine($" [x] {url} is not a federation instance");
                // valid URL, but not a federation instance. Handle this case.
            }
        }
        else
        {
            Console.WriteLine($"[x] {url} is not a valid HTTPS URL");
            // invalid URL. Handle this case.
        }
    }
    
    
}
public class RazorBlade
{
    static IPlaywright playwright;
    static IBrowser browser;
    
    public async void GiveBirth()
    {
        RazorBlade.playwright = await Playwright.CreateAsync();
        RazorBlade.browser = await playwright.Webkit.LaunchAsync();
    }

    public static async Task<string> Scrape(string url, string platform)
    {
        switch (platform)
        {
            case "Lemmy":
                return await LemmyScrape(url); // This is also when we store it into the redis database
            case "Kbin":
                return await KbinScrape(url); // This is also when we store it into the redis database
            default:
                return "No platform found"; // Impossible case
        }
    }
    
    static async Task<string> KbinScrape(string url) {
        
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url);
        var x = page.Locator("#content > div");
        var y = x.Locator("article");
        return y.ToString();
    }
    
    static async Task<string> LemmyScrape(string url) {
        
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url);
        var x = page.Locator("#app > div.mt-4.p-0.fl-1 > div > div > div > main > div.main-content-wrapper > div > div.post-listings > div:nth-child(1)");
        
        return x.ToString();
    }
}
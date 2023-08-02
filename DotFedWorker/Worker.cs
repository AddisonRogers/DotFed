using System.Text;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var razorBlade = new RazorBlade();
        razorBlade.GiveBirth();
        
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
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        
        channel.BasicConsume(queue: "hello",
            autoAck: false,
            consumer: consumer);
        
        
        while (!stoppingToken.IsCancellationRequested)
        {
            
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            Thread.Sleep(1000);
        }
    }
}


public class RazorBlade
{
    IPlaywright playwright;
    IBrowser browser;
    
    public async void GiveBirth()
    {
        this.playwright = await Playwright.CreateAsync();
        this.browser = await playwright.Webkit.LaunchAsync();
    }
    
    async Task<string> LemmyScrape(string url) {
        
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url);
        var x = page.Locator("#app > div.mt-4.p-0.fl-1 > div > div > div > main > div.main-content-wrapper > div > div.post-listings > div:nth-child(1)");
        
        return x.ToString();
    }
}
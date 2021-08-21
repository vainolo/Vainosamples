using Microsoft.Extensions.Logging;

class Program 
{
    public static void Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogInformation("Hello World!");
    }
}
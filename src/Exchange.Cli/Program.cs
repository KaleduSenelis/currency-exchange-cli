using Exchange.Application;
using Exchange.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Exchange.Cli;

internal class Program
{
    static async Task Main(string[] args)
    {
        using var tokenSource = new CancellationTokenSource();

        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile("appsettings.json");
        });

        builder.ConfigureServices(services =>
        {
            services.AddInfrastructure();
            services.AddApplication();
            services.AddScoped<App>();
        });
        
        var host = builder.Build();

        Console.CancelKeyPress += (_, cancelEventArguments) =>
        {
            cancelEventArguments.Cancel = true;
            tokenSource.Cancel();
        };

        await host.StartAsync(tokenSource.Token);

        using var scope = host.Services.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<App>();

        await app.RunAsync(tokenSource.Token);

        await host.StopAsync(tokenSource.Token);
    }
}

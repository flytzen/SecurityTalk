using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Web
{
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ConfigureLogging(Configuration);
                BuildWebHost(new string[0]{}).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NOTHING"}.json",
                optional: true)
            //.AddUserSecrets<Startup>()
            .AddEnvironmentVariables()
            .Build();

        private static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel(c => c.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(Configuration)
                .UseApplicationInsights()
                .UseSerilog()
                .UseStartup<Startup>()
                .Build();
        }

        private static void ConfigureLogging(IConfiguration configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", environment);

            if (environment.ToUpperInvariant() == "DEVELOPMENT")
            {
                loggerConfiguration.WriteTo.ColoredConsole();
            }
            else
            {
                loggerConfiguration
                    .WriteTo.ApplicationInsightsTraces(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Information("Logging initialised");
        }
    }
}

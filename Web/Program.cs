using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Web
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Configuration.AzureKeyVault;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ConfigureLogging(Configuration);
                try
                {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();

                    var keyvault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    var t = keyvault.GetSecretsAsync("https://securitytalkvault.vault.azure.net/").Result;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to play with keyvault");
                }
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

        public static IConfiguration Configuration
        {
            get
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

                // Integrate with Managed Identity
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                //// Get a link to the keyvault
                //var keyvault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)); 

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)
                    .AddUserSecrets<Startup>()
                    
                    .AddEnvironmentVariables();

                //if (!environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
                //{
                //    // Add key vault as a configuration provider
                //    builder.AddAzureKeyVault("https://securitytalkvault.vault.azure.net/", keyvault,
                //        new DefaultKeyVaultSecretManager());
                //}

                return builder.Build();

            }
        }

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    using Db;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.AzureKeyVault;
    using Serilog;

    public class HomeController : Controller
    {
        private MyContext context;
        private readonly IConfiguration configuration;

        public HomeController(MyContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = new HomeModel()
            {
                Customers = this.context.Customers.ToList(),
                DummyConfigValue = this.configuration["Dummy"]
            };

            return View(model);
        }

        public IActionResult About()
        {
            try
            {
                Log.Information("Starting to play with key vault");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                var keyvault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                Log.Information("Reading secrets");
                var t = keyvault.GetSecretsAsync("https://securitytalkvault.vault.azure.net/").Result;
                Log.Information("Read {count} secrets", t.Count());
                var t2 = new ConfigurationBuilder().AddAzureKeyVault("https://securitytalkvault.vault.azure.net/",
                    keyvault, new DefaultKeyVaultSecretManager());
                Log.Information("Added keyvautl to a builder");
                var t3 = t2.Build();
                Log.Information("Built the thing");
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to play with keyvault");
            }
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

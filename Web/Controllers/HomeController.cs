using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    using System.IO;
    using Db;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.AzureKeyVault;
    using Microsoft.Net.Http.Headers;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
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

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public async Task<IActionResult> GetFile()
        {
            try
            {
                // This gets the connection string - but actually from KeyVault and not from config
                var storageConnectionString = this.configuration["StorageConnectionString"];

                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var containerReference = blobClient.GetContainerReference("mycontainer");
                if (await containerReference.CreateIfNotExistsAsync())
                {
                    // Initialise for first use 
                    Log.Information("Initialising blob storage");
                    var uploadblobReference = containerReference.GetBlockBlobReference("myblob.txt");
                    await uploadblobReference.UploadTextAsync("My file content");
                }

                var blobReference = containerReference.GetBlockBlobReference("myblob.txt");
                var ms = new MemoryStream();
                await blobReference.DownloadToStreamAsync(ms);
                ms.Position = 0;
                return new FileStreamResult(ms, "text/plain")
                {
                    FileDownloadName = "myblob.txt"
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to play blob storage");
                throw;
            }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideEncryption.BlobDemo
{
    using System.IO;
    using System.Threading;
    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class BlobDemo
    {
        // This should of course be read from the app settings we should have been getting from Key Vault using MSI!
        private readonly string storageConnectionString;

        private readonly KeyVaultKeyResolver keyResolver;

        public BlobDemo(string storageConnectionString)
        {
            this.storageConnectionString = storageConnectionString;

            // This creates a "key resolver" that we can use to to encrypt and decrypt blobs
            keyResolver = new KeyVaultKeyResolver(GetAuthToken);
        }

        public async Task Run()
        {
            await Initialise();
            await UploadBlob();
            await DownloadBlob();
        }

        private async Task UploadBlob()
        {
            Console.WriteLine("Uploading...");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference("consoleappcontainer");
            var blobReference = containerReference.GetBlockBlobReference("myblob.txt");
            var encryptionKey = await this.keyResolver.ResolveKeyAsync("https://securitytalkvault.vault.azure.net/keys/BlobEncryption1", new CancellationToken());

            await blobReference.UploadTextAsync(
                "My file content", 
                Encoding.UTF8,
                AccessCondition.GenerateIfNotExistsCondition(), 
                new BlobRequestOptions()
                {
                    EncryptionPolicy = new BlobEncryptionPolicy(encryptionKey, null)
                },
                null);
        }

        private async Task DownloadBlob()
        {
            Console.WriteLine("Downloading...");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference("consoleappcontainer");
            var blobReference = containerReference.GetBlockBlobReference("myblob.txt");
            var blobContent = await blobReference.DownloadTextAsync(
                Encoding.UTF8,
                AccessCondition.GenerateIfExistsCondition(), 
                new BlobRequestOptions()
                {
                    EncryptionPolicy = new BlobEncryptionPolicy(null, keyResolver)
                }, 
                null);
            Console.WriteLine(blobContent);
        }

        private async Task Initialise()
        {
            Console.WriteLine("Initialising...");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerReference = blobClient.GetContainerReference("consoleappcontainer");
            await containerReference.CreateIfNotExistsAsync();
        }


        private async Task<string> GetAuthToken(string authority, string resource, string scope)
        {
            // Using keys from config, but should really be Managed Service Identity
            var configSection = Program.Configuration.GetSection("KeyVault");
            var clientCredential = new ClientCredential(configSection["ApplicationId"], configSection["Key"]);

            var authContext = new AuthenticationContext(authority);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCredential);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the access token");
            return result.AccessToken;
        }
    }
}

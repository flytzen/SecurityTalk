using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideEncryption.DbDemo
{
    using System.Data.SqlClient;
    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider;

    public class Demo
    {
        private readonly string baseConnectionString;

        static Demo()
        {
            TellEFToUseKeyVaultForConnections(); 
        }

        public Demo(string baseConnectionString)
        {
            this.baseConnectionString = baseConnectionString;
            Seed();
        }

        public void Run()
        {
            using (var context = this.CreateContext())
            {
                Console.WriteLine("Retrieving...");
                foreach (var customer in context.Customers)
                {
                    Console.WriteLine($"{customer.Id}\t{customer.Name}");
                }
            }
        }

        private void Seed()
        {
            using (var context = this.CreateContext())
            {
                if (!context.Customers.Any())
                {
                    Console.WriteLine("Seeding...");
                    // Never use constants when editing/inserting encrypted columns:
                    // They are not parameterised so won't be encrypted and will throw an exception.
                    var customerName1 = "Ajax inc";
                    context.Customers.Add(new Customer() { Name = customerName1 });
                    var customerName2 = "Big Evil Corp";
                    context.Customers.Add(new Customer() { Name = customerName2 });
                    context.SaveChanges();
                }
            }
        }

        private MyContext CreateContext()
        {
            // Add Use Column Encryption to the connection string. Or just do that in config.
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled
            };

            return new MyContext(builder.ConnectionString);
        }

        private static void TellEFToUseKeyVaultForConnections()
        {
            var keyProvider = GetKeyProvider(); // Use Azure Key vault to get the encryption keys
            var providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>
            {
                {SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, keyProvider}
            };
            // This is a static method
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
        }

        private static SqlColumnEncryptionAzureKeyVaultProvider GetKeyProvider()
        {
            
            var keyProvider = new SqlColumnEncryptionAzureKeyVaultProvider(GetAuthToken);
            return keyProvider;
        }

        private static async Task<string> GetAuthToken(string authority, string resource, string scope)
        {
            // Using keys from config, but should ideally be Managed Service Identity
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

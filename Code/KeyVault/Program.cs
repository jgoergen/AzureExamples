using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace KeyVault
{
    class Program
    {
        private const string applicationId = "3d3409f9-b394-4aef-81cf-d010686dfa36";
        private const string applicationSecret = "cvNsKomLZRUexFW9qso0ZUBNcH58atyiYhn8nq8mCFU=";
        private const string secretidentifier = "https://testvault123jg.vault.azure.net/secrets/testsecret/c40698ae9895495aa14fbd9bcfca541d";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Azure Storage Blob Examples");

            Task t = StartMainAsync();
            t.Wait();

            Console.WriteLine($"Examples completed, press enter to quit.");
            Console.ReadLine();
        }

        static async Task StartMainAsync()
        {
            Console.WriteLine("Connecting to active directory");

            var keyClient = new KeyVaultClient(GetToken);

            Console.WriteLine("Getting secret from vault");

            // Get the secret details

            var secret = await keyClient.GetSecretAsync(secretidentifier);

            Console.WriteLine($"secret is {secret}");
        }

        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(applicationId, applicationSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}

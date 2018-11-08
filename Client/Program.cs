using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        
        private static async Task MainAsync()
        {
            // discover endpoints from the metadata by calling Auth server hosted on 5000 port
            var discoveryClient = await DiscoveryClient.GetAsync("http://localhost:5001");
            if (discoveryClient.IsError)
            {
                Console.WriteLine(discoveryClient.Error);
                return;
            }
            // request the token from the Auth server
            var tokenClient = new TokenClient(discoveryClient.TokenEndpoint, "client", "secret");
            var response = await tokenClient.RequestClientCredentialsAsync("api1");
 
            if (response.IsError)
            {
                Console.WriteLine(response.Error);
                return;
            }
 
            Console.WriteLine(response.Json);
        }
    }
}
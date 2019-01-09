using System;
using System.ComponentModel.Design;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Converters;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
            //MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            // discover endpoints from the metadata by calling Auth server hosted on 5000 port
            // request the token from the Auth server
            //Console.WriteLine(disco.TokenEndpoint);
            //var tokenClient = new TokenClient(disco.TokenEndpoint, "eada14e8-b238-458e-a0d9-b2f913c0289d", "test123");
            //Console.WriteLine(tokenClient.ClientId);
            //var response = await tokenClient.RequestClientCredentialsAsync("api1");

            var clientId = "129d4141-0f71-42f7-b4c2-8bb53a3b3e46";

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/hubs/chatchain",
                    options => { options.AccessTokenProvider = () => GetJwtToken(clientId, "test123"); })
                .Build();
            
            Console.WriteLine("test1234");

            await connection.StartAsync();
            
            connection.On<string, string, string, string, string>("GenericMessageEvent",
                (clientType, clientName, channel, user, message) =>
                {
                    Console.WriteLine(
                        $"Client Name: {clientType} ---- Group Name: {clientName} ---- Group Id: {channel}");
                });

            Console.WriteLine("test123");
            
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };

            while (connection.State == HubConnectionState.Disconnected)
            {
                Thread.Sleep(1);
            }
            //await connection.InvokeAsync("GenericMessageEvent", "1", "2", "3", "4", "5");
            while (true)
            {
                Thread.Sleep(10000);
                Console.WriteLine("");
                await connection.InvokeAsync("GenericMessageEvent", clientId, "2", "3", "4", "5");
            }
        }

        private static async Task<String> GetJwtToken(string userid, string password)
        {
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
            if (disco.IsError) throw new Exception(disco.Error);

            var response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "client_credentials",

                ClientId = userid, //"71ed33b6-92e3-481a-8bab-488d07d69494",
                ClientSecret = password, //"test123",

                Parameters =
                {
                    {"scope", "api1"}
                }
            });
            
            if (response.IsError)
            {
                Console.WriteLine(response.Error);
            }
            
            return response.Json.GetValue("access_token").ToString();
        }
    }
}
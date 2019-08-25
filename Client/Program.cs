using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

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
           
            const string clientId = "5bd764d8-cad2-4468-a72c-5771f781fed8";
            const string clientSecret = "U5FrQ1xYJ@pk6?KH$F_6aodS7W01-c6$?81!89w5a?LY@zeNA$1gk$S@4$Kq?X";

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/hubs/chatchain",
                    options => { options.AccessTokenProvider = () => GetJwtToken(clientId, clientSecret); })
                .Build();
            
            Console.WriteLine("test1234");

            await connection.StartAsync();
            
            connection.On<JObject>("ReceiveGenericMessage",
                message =>
                {
                    Console.WriteLine(
                        $"{message}");
                    Console.WriteLine("");
                });

            Console.WriteLine("test123");
            
            connection.Closed += async error =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };

            while (connection.State == HubConnectionState.Disconnected)
            {
                Thread.Sleep(1);
            }
            await connection.InvokeAsync("SendGenericMessage", new GenericMessage
            {
                Message = "TESTING",
                Group = new Group
                {
                    GroupId = "f655f1d7-e54e-4ce9-882f-d65c09a5825d"
                },
                User = new User
                {
                    Name = "TestingUser",
                    ClientRanks = new List<ClientRank>()
                }
            });
            int i = 0;
            while (i < 10)
            {
                i++;
                Thread.Sleep(10000);
            }
        }

        private static async Task<string> GetJwtToken(string userid, string password)
        {
            HttpClient client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001");
            if (disco.IsError) throw new Exception(disco.Error);

            TokenResponse response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "client_credentials",

                ClientId = userid, //"71ed33b6-92e3-481a-8bab-488d07d69494",
                ClientSecret = password, //"test123",

                Parameters =
                {
                    {"scope", "ChatChain"}
                }
            });
            
            if (response.IsError)
            {
                Console.WriteLine(response.Error);
            }
            
            return response.Json.GetValue("access_token").ToString();
        }
    }
    
    public class Group
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string OwnerId { get; set; }
        public string GroupDescription { get; set; }
    }
    
    public class ClientRank
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public int Priority { get; set; }
        public string Display { get; set; }
        public string Colour { get; set; }
    }
    
    public class User
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
        public string NickName { get; set; }
        public string Colour { get; set; }
        public List<ClientRank> ClientRanks { get; set; }
        
    }
    
    public class Client
    {
        public string ClientId { get; set; }
        public string OwnerId { get; set; }
        public string ClientGuid { get; set; }
        public string ClientName { get; set; }
        public string ClientDescription { get; set; }
    }
    
    public class GenericMessage
    {
        public Group Group { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public Client SendingClient { get; set; }
        public bool SendToSelf { get; set; }
    }
}
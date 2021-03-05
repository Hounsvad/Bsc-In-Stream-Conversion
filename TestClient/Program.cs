
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        private static List<HubConnection> conns = new List<HubConnection>();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            for (int i = 0; i < 50; i++)
            {
                i = await CreateClient(i);
            }
            var bob = Console.ReadLine();
        }

        private static async Task<int> CreateClient(int i)
        {
            Console.WriteLine($"Live clients: {conns.Count + 1}");
            return await await Task<Task<int>>.Factory.StartNew(async () =>
            {
                HubConnection conn = new HubConnectionBuilder()
               .WithAutomaticReconnect()
               .WithUrl("https://home.hounsvad.dk:44380/SubscribeHub", (opts) =>
               {
                   opts.HttpMessageHandlerFactory = (message) =>
                   {
                       if (message is HttpClientHandler clientHandler)
                           // bypass SSL certificate
                           clientHandler.ServerCertificateCustomValidationCallback +=
                           (sender, certificate, chain, sslPolicyErrors) => { return true; };
                       return message;
                   };
               })
               .Build();

                conns.Add(conn);

                conn.On<string>("NewData", data =>
                {
                    //Console.WriteLine(data);
                });

                try
                {
                    await conn.StartAsync();
                    //Console.WriteLine("Connection started");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    conns.Remove(conn);
                    return i--;
                }

                try
                {
                    await conn.InvokeAsync("SubscribeTo", "Hounsvad%2Fpi%2Fcputemp%2FDEG_C", "K");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    conns.Remove(conn);
                    return i--;
                }
                return i;
            });
        }
    }
}

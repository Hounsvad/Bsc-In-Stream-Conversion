
using Common;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
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
        private static bool currentlyMeasuring = false;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            while(conns.Count < 50) { 
                await CreateClient();
            }
            string input = "";
            while(input != "exit")
            {
                input = Console.ReadLine().ToLowerInvariant();
                switch (input)
                {
                    case "start": currentlyMeasuring = true;break;
                    case "stop": currentlyMeasuring = false;break;
                    case "dump": PerformanceMeasurer.DumpLog(); break;
                    case "help": Console.WriteLine("start: Starts measuring incoming data\n" +
                                                    "stop: Stops measuring incoming data\n" +
                                                    "dump: Writes all measurements to a file\n" +
                                                    "exit: Exits the program");break;
                    default: break;
                }
            }
            
        }

        private static async Task CreateClient()
        {
            Console.WriteLine($"Live clients: {conns.Count + 1}");
            await Task.Run(async () =>
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
                    if (currentlyMeasuring)
                    {
                        PerformanceMeasurer.Log(JsonConvert.DeserializeObject<ClientMessageDto>(data), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                    }
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
                }

                if (conn.State == HubConnectionState.Connected)
                {
                    return;
                }
                else
                {
                    conns.Remove(conn);
                }
            });
        }
    }
}

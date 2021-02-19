
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        private static List<HubConnection> conns = new List<HubConnection>();
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine($"Live clients: {conns.Count}");
                await Task.Run(async () =>
                {
                    HubConnection conn = new HubConnectionBuilder()
                   .WithAutomaticReconnect()
                   .WithUrl("https://localhost:44380/SubscribeHub")
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
                    }

                    try
                    {
                        await conn.InvokeAsync("SubscribeTo", "Hounsvad%2Fpi%2Fcputemp%2FDEG_C", "K");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                });
                Thread.Sleep(1000);
                if(conns.Count % 10 == 0)
                {
                    Thread.Sleep(10000);
                }
            }
        }
    }
}

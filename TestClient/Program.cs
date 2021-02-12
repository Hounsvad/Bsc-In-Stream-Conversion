
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        private static HubConnection conn;
        static async Task Main(string[] args)
        {
            conn = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl("https://localhost:44380/SubscribeHub")
                .Build();

            conn.On<string>("NewData", data =>
            {
                Console.WriteLine(data);
            });

            try
            {
                await conn.StartAsync();
                Console.WriteLine("Connection started");
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
            while (true) ;
        }
    }
}

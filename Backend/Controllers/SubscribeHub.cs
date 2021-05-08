using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Bsc_In_Stream_Conversion.Controllers
{
    public class SubscribeHub : Hub
    {
        private IStreamClientManager mqttClientManager;
        private SocketRequestHandler socketRequestHandler;
        private static int counter = 0;


        public SubscribeHub(IStreamClientManager mqttClientManager, SocketRequestHandler socketRequestHandler)
        {
            this.mqttClientManager = mqttClientManager;
            this.socketRequestHandler = socketRequestHandler;
        }

        public async Task SubscribeTo(string Topic, string ToUnit)
        {
            try
            {
                var topicTranslated = Topic.Replace("%2F", "/");
                await socketRequestHandler.Subscribe(topicTranslated, ToUnit, Clients.Caller.SendCoreAsync);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            socketRequestHandler.Unsubscribe();
            return base.OnDisconnectedAsync(exception);
        }


        public override Task OnConnectedAsync()
        {
            ++counter;
            Console.WriteLine("Counter: " + counter);
            Log.Debug("Counter: " + counter);
            return base.OnConnectedAsync();
        }
    }
}

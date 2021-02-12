using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion.Controllers
{
    public class SubscribeHub : Hub
    {
        private IMQTTClientManager mqttClientManager;
        private SocketRequestHandler socketRequestHandler;
        

        public SubscribeHub(IMQTTClientManager mqttClientManager, SocketRequestHandler socketRequestHandler)
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
    }
}

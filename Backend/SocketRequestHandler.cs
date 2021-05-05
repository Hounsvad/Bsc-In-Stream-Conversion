using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Bsc_In_Stream_Conversion
{
    public class SocketRequestHandler
    {
        private List<string> messages = new List<string>();
        private Guid subscribtionId;
        private UserUnit FromUnit;

        private string topic;
        private UserUnit toUnit;
        private IStreamClientManager mqttClientManager;
        private Func<string, object[], CancellationToken, Task> answerCallback;
        private IUnitConverter unitConverter;
        private readonly UnitFactory unitFactory;

        public SocketRequestHandler(IStreamClientManager mqttClientManager, IUnitConverter unitConverter, UnitFactory unitFactory)
        {
            this.mqttClientManager = mqttClientManager;
            this.unitConverter = unitConverter;
            this.unitFactory = unitFactory;
        }

        public async Task Subscribe(string topic, string toUnit, Func<string, object[], CancellationToken, Task> answerCallback)
        {
            this.topic = topic;
            this.toUnit = await unitFactory.Parse(toUnit);
            this.answerCallback = answerCallback;
            FromUnit = await unitFactory.Parse(topic.Split("/").Last().Replace("47", "/"));
            subscribtionId = await mqttClientManager.Subscribe(topic, HandleNewMessage);
        }

        internal void Unsubscribe()
        {
            mqttClientManager.Unsubscribe(subscribtionId);
        }

        private async Task HandleNewMessage(string message)
        {
            try
            {
                ReadingDto msg = JsonConvert.DeserializeObject<ReadingDto>(message);

                var convertedValue = toUnit.ConvertFromBaseValue(FromUnit.ConvertToBaseValue(msg.Reading));

                //Changed to accomodate new testclient
                //await answerCallback("NewData", new object[] { convertedValue.ToString() }, CancellationToken.None);
                var clientMessage = new ClientMessageDto(msg, Thread.CurrentThread.ManagedThreadId, mqttClientManager.GetCurrentThreadCount(), convertedValue);
                await answerCallback("NewData", new object[] { JsonConvert.SerializeObject(clientMessage) }, CancellationToken.None);
            }catch(Exception e)
            {
                Log.Error("Error sending message " + e.StackTrace, e);
            }
        }
    }
}

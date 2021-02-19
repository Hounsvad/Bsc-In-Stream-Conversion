using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class SocketRequestHandler
    {
        private List<string> messages = new List<string>();
        private Guid subscribtionId;
        private UserUnit FromUnit;

        private string topic;
        private string toUnit;
        private IMQTTClientManager mqttClientManager;
        private Func<string, object[], CancellationToken, Task> answerCallback;
        private IUnitConverter unitConverter;
        private readonly UnitFactory unitFactory;

        public SocketRequestHandler(IMQTTClientManager mqttClientManager, IUnitConverter unitConverter, UnitFactory unitFactory)
        {
            this.mqttClientManager = mqttClientManager;
            this.unitConverter = unitConverter;
            this.unitFactory = unitFactory;
        }

        public async Task Subscribe(string topic, string toUnit, Func<string, object[], CancellationToken, Task> answerCallback)
        {
            this.topic = topic;
            this.toUnit = toUnit;
            this.answerCallback = answerCallback;
            FromUnit = await unitFactory.Parse(topic.Split("/").Last().Replace("%F2", "/"));
            subscribtionId = await mqttClientManager.Subscribe(topic, HandleNewMessage);
        }

        private async Task HandleNewMessage(string message)
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                var userUnit = await unitFactory.Parse(toUnit);
                var value = decimal.Parse(message, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                var convertedValue = userUnit.ConvertFromBaseValue(FromUnit.ConvertToBaseValue(value));

                await answerCallback("NewData", new object[] { convertedValue.ToString() }, CancellationToken.None);
                timer.Stop();
                Log.Information($"{timer.ElapsedMilliseconds}");
            }catch(Exception e)
            {
                Log.Error("Error sending message " + e.StackTrace, e);
            }
        }
    }
}

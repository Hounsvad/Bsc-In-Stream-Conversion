using System;
using System.Collections.Generic;
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

        public SocketRequestHandler(IMQTTClientManager mqttClientManager, IUnitConverter unitConverter)
        {
            this.mqttClientManager = mqttClientManager;
            this.unitConverter = unitConverter;
        }

        public async Task Subscribe(string topic, string toUnit, Func<string, object[], CancellationToken, Task> answerCallback)
        {
            this.topic = topic;
            this.toUnit = toUnit;
            this.answerCallback = answerCallback;
            FromUnit = UserUnit.Parse(topic.Split("/").Last().Replace("%F2", "/"));
            subscribtionId = await mqttClientManager.Subscribe(topic, HandleNewMessage);
        }

        private async Task HandleNewMessage(string message)
        {
            var userUnit = UserUnit.Parse(toUnit);
            var value = decimal.Parse(message, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            var numeratorValue = await unitConverter.Convert(FromUnit.Numerator, userUnit.Numerator, value);
            var denominatorValue = await unitConverter.Convert(FromUnit.Denominator, userUnit.Denominator, 1);

            var convertedValue = numeratorValue / denominatorValue;

            await answerCallback("NewData", new object[]{ convertedValue.ToString() }, CancellationToken.None);
        }
    }
}

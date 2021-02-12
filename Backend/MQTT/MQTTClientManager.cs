using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class MQTTClientManager : IMQTTClientManager
    {
        private IMqttClient client;
        private IMqttClientOptions options;
        private const int MAX_RECONNECT_ATTEMPTS = 10;
        private int reconnectAttempts = 0;

        //This is not the most CPU efficient way to store this
        private Dictionary<Guid, (string, Func<string, Task>)> TopicsCallbacks = new Dictionary<Guid, (string, Func<string, Task>)>();

        public MQTTClientManager()
        {
            SetupConenction("home.hounsvad.dk", 1883);
        }

        private void SetupConenction(string url, int port)
        {
            reconnectAttempts = 0;
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();

            options = new MqttClientOptionsBuilder()
                .WithClientId("Automatic unit converter")
                .WithTcpServer(url, port)
                .WithCleanSession()
                .Build();

            client.UseDisconnectedHandler(async e =>
            {
                if (reconnectAttempts >= MAX_RECONNECT_ATTEMPTS) return;

                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await client.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
                }
                catch(Exception ex)
                {
                    reconnectAttempts++;
                }
            });

            client.UseApplicationMessageReceivedHandler(e =>
            {
                foreach(var (x, y) in TopicsCallbacks.Values.Where(x => x.Item1 == e.ApplicationMessage.Topic))
                {
                    y(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                }
            });
        }

        public async Task<Guid> Subscribe(string topic, Func<string, Task> messageCallback )
        {
            if (!client.IsConnected)
            {
                await client.ConnectAsync(options);
            }
            var result = await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            var subId = Guid.NewGuid();
            TopicsCallbacks.Add(subId, (topic, messageCallback));
            return subId;
        }

        public async Task Unsubscribe(Guid subId)
        {
            var topic = TopicsCallbacks[subId].Item1;
            TopicsCallbacks.Remove(subId);
            if (TopicsCallbacks.Values.Where(x => x.Item1 == topic).Count() == 0)
            {
                await client.UnsubscribeAsync(topic);
            }
        }

        public async Task PublishMessageAsync(string topic, string message)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("MyTopic")
                                .WithPayload("Hello World")
                                .WithExactlyOnceQoS()
                                .WithRetainFlag()
                                .Build();

            await client.PublishAsync(mqttMessage, CancellationToken.None);
        }
    }
}

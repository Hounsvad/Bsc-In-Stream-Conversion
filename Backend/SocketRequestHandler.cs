﻿using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;

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

        //private static ConcurrentQueue<(int, long)> ResponseTimeByLiveClients = new ConcurrentQueue<(int, long)>();

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
            try
            {
                //Stopwatch timer = new Stopwatch();
                //timer.Start();
                var msgInObj = JsonConvert.DeserializeObject<ReadingDto>(message);
                var userUnit = UserUnit.Parse(toUnit);
                var value = msgInObj.Reading;

                var numeratorValue = await unitConverter.Convert(FromUnit.Numerator, userUnit.Numerator, value);
                var denominatorValue = await unitConverter.Convert(FromUnit.Denominator, userUnit.Denominator, 1);

                var fromUnitPrefixfactor = (decimal)Math.Pow(FromUnit.NumeratorPrefixes.Base, FromUnit.NumeratorPrefixes.Factor) /
                                                (decimal)Math.Pow(FromUnit.DenominatorPrefixes.Base, FromUnit.DenominatorPrefixes.Factor);

                var toUnitPrefixfactor = (decimal)Math.Pow(userUnit.DenominatorPrefixes.Base, userUnit.DenominatorPrefixes.Factor) /
                                        (decimal)Math.Pow(userUnit.NumeratorPrefixes.Base, userUnit.NumeratorPrefixes.Factor);

                var convertedValue = fromUnitPrefixfactor * toUnitPrefixfactor * numeratorValue / denominatorValue;

                var clientMessage = new ClientMessageDto(msgInObj, Thread.CurrentThread.ManagedThreadId, mqttClientManager.GetActiveClientsCount(), convertedValue);

                await answerCallback("NewData", new object[] { JsonConvert.SerializeObject(clientMessage) }, CancellationToken.None);

                //timer.Stop();
                //ResponseTimeByLiveClients.Enqueue((mqttClientManager.GetActiveClientsCount(), timer.ElapsedMilliseconds));
            }catch(Exception e)
            {
                Log.Error("Error sending message " + e.StackTrace, e);
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace TestClient
{
    public class PerformanceMeasurer
    {
        private static int NumberOfLogFiles = 0;
        public static string StartTime = "";
        private static ConcurrentQueue<(Guid, ClientMessageDto)> log = new ConcurrentQueue<(Guid, ClientMessageDto)>();
        private static bool IsRunning = false;
        private static Semaphore _lock = new Semaphore(1,1);
        public static int dumpnr = 0;

        public static void Log(ClientMessageDto msg, long timeOfRetrieval, Guid clientId)
        {
            msg.TimeOfReading = timeOfRetrieval - msg.TimeOfReading;
            _lock.WaitOne();
            log.Enqueue((clientId, msg));
            _lock.Release();
        }

        public static async Task DumpLog()
        {
            try
            {
                if (IsRunning) return;
                IsRunning = true;
                //using (var fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/UnitPerformance/{StartTime}Dump{++NumberOfLogFiles}.txt"))
                using (var fs = new StreamWriter(new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/UnitPerformance/{StartTime}ClientDump{dumpnr:000}.txt", FileMode.OpenOrCreate)))
                {
                    _lock.WaitOne();
                    var array = log.ToList();
                    
                    var missing = FindMissing(array);

                    _lock.Release();
                    fs.Write($"Tickrate: {Stopwatch.Frequency} Measurement Count: {array.Count} MissingMessages: {missing}\n");
                    foreach (var entry in array)
                    {
                        fs.Write($"NumberOfThreads:{entry.Item2.NumberOfThreads}:Time:{entry.Item2.TimeOfReading}:ThreadId:{entry.Item2.ThreadId}:ReadingId:{entry.Item2.ReadingId}\n");
                    }
                    fs.Flush();
                    fs.Close();
                }
                IsRunning = false;
                Console.WriteLine("Done dumping\a");
            }
            catch (Exception e)
            {
                _lock.Release();
                Console.WriteLine();
                Console.WriteLine(e.Message + ":" + e.StackTrace);
                Console.WriteLine();
            }
        }

        private static int FindMissing(List<(Guid, ClientMessageDto)> msgsByClients)
        {
            Dictionary<Guid, List<ClientMessageDto>> dict = new Dictionary<Guid, List<ClientMessageDto>>();
            foreach (var msg in msgsByClients)
            {
                if (!dict.TryAdd(msg.Item1, new List<ClientMessageDto>() {msg.Item2}))
                {
                    dict[msg.Item1].Add(msg.Item2);
                }
            }

            log = new ConcurrentQueue<(Guid, ClientMessageDto)>();
            int missing = 0;
            foreach (var msgsByClient in dict)
            {
                for (int i = 1; i < msgsByClient.Value.Count; i++)
                {
                    if (msgsByClient.Value[i].MessageCounterId != ++msgsByClient.Value[i - 1].MessageCounterId)
                    {
                        missing++;
                    }
                }
            }

            return missing;
        }
    }
}

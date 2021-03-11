using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    public class PerformanceMeasurer
    {
        private static int NumberOfLogFiles = 0;
        public static string StartTime = "";
        private static ConcurrentQueue<ClientMessageDto> log = new ConcurrentQueue<ClientMessageDto>();
        private static bool IsRunning = false;
        private static Semaphore _lock = new Semaphore(1,1);
        public static int dumpnr = 0;

        public static void Log(ClientMessageDto msg, long timeOfRetrieval)
        {
            msg.TimeOfReading = timeOfRetrieval - msg.TimeOfReading;
            _lock.WaitOne();
            log.Enqueue(msg);
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
                    var array = log.ToArray();
                    log = new ConcurrentQueue<ClientMessageDto>();
                    _lock.Release();
                    fs.Write("Tickrate: " + Stopwatch.Frequency + " Measurement Count: "+array.Length + "\n");
                    foreach (var entry in array)
                    {
                        fs.Write($"NumberOfThreads:{entry.NumberOfThreads}:Time:{entry.TimeOfReading}:ThreadId:{entry.ThreadId}:ReadingId:{entry.ReadingId}\n");
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
    }
}

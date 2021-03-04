using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bsc_In_Stream_Conversion
{
    public class PerformanceMeasurer
    {
        private static int NumberOfLogFiles = 0;
        public static string StartTime = "";
        private static ConcurrentQueue<Entry> log = new ConcurrentQueue<Entry>();
        private static bool IsRunning = false;

        public static void Log(int NumberOfThreads, long Ticks, int ThreadId)
        {
            var e = new Entry()
            {
                NumberOfThreads = NumberOfThreads,
                Ticks = Ticks,
                ThreadId = ThreadId
            };
            log.Enqueue(e);
        }

        public static async Task DumpLog()
        {
            if (IsRunning) return;
            IsRunning = true;
            await Task.Delay(60000);
            using (var fs = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/UnitPerformance/{StartTime}Dump{++NumberOfLogFiles}.txt"))
            {
                var array = log.ToArray();
                log = new ConcurrentQueue<Entry>();
                foreach (var entry in array)
                {
                    var byteArray = Encoding.UTF8.GetBytes($"NumberOfThreads:{entry.NumberOfThreads}:Ticks:{entry.Ticks}:ThreadId:{entry.ThreadId}");
                    fs.Write(byteArray, 0, byteArray.Length);
                }
                fs.Flush();
                fs.Close();
            }
            IsRunning = false;
        }

        private class Entry
        {
            public int NumberOfThreads { get; set; }
            public long Ticks { get; set; }
            public int ThreadId { get; set; }
        }
    }
}

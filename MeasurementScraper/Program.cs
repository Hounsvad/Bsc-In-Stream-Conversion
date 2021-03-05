using System;
using System.IO;
using System.Linq;

namespace MeasurementScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var InputDirectoryPath = args[0];
            var OutputDirectoryPath = args[1];
            using (var sw = new StreamWriter(new FileStream(OutputDirectoryPath + "\\combinedMeasurementOutput.txt", FileMode.OpenOrCreate))) {
                
                var files = Directory.EnumerateFiles(InputDirectoryPath, "*.txt");
                sw.WriteLine("Threadcount,Ticks,ThreadID");
                sw.Flush();
                var filecount = 0;
                foreach (string file in files)
                {
                    Console.WriteLine($"Reading file {++filecount} out of {files.Count()} + Name: {file}");
                    using (var sr = new StreamReader(new FileStream(file, FileMode.Open)))
                    {
                        sr.ReadLine();
                        while(!sr.EndOfStream){
                            string line = sr.ReadLine();
                            var entries = line.Split(':');
                            if(entries.Length>=5 && int.Parse(entries[1])%50 == 0)
                            {
                                sw.WriteLine($"{entries[1]},{entries[3]},{entries[5]}");
                            }
                        }
                        sw.Flush();
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MeasurementScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                Dictionary<int, List<string>> entriesList = new Dictionary<int, List<string>>();
                var InputDirectoryPath = args[0];
                var OutputDirectoryPath = args[1];
                var OutputFileName = args.Length >= 3 ? args[2] : "combinedMeasurementOutputClientMeasurement.csv";
                if(args.Length == 3)
                using (var sw = new StreamWriter(new FileStream(OutputDirectoryPath + OutputFileName , FileMode.OpenOrCreate)))
                {

                    var files = Directory.EnumerateFiles(InputDirectoryPath, "*.txt");
                    sw.WriteLine("Threadcount,Milliseconds,ThreadID,ReadingID");
                    sw.Flush();
                    var filecount = 0;
                    foreach (string file in files)
                    {
                        Console.WriteLine($"Reading file {++filecount} out of {files.Count()} + Name: {file}");
                        using (var sr = new StreamReader(new FileStream(file, FileMode.Open)))
                        {
                            sr.ReadLine();
                            while (!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                var entries = line.Split(':');
                                if (entries.Length >= 8)
                                {
                                    int key = int.Parse(entries[1]);
                                    if (entriesList.ContainsKey(key))
                                    {
                                        entriesList[key].Add($"{entries[1]},{entries[3]},{entries[5]},{entries[7]}");
                                    }
                                    else
                                    {
                                        entriesList.Add(key, new List<string>() { $"{entries[1]},{entries[3]},{entries[5]},{entries[7]}" });
                                    }
                                }
                            }
                        }
                    }

                    foreach (var e in entriesList)
                    {
                        Console.WriteLine($"Writing {e.Key}s");
                        foreach (var entry in e.Value)
                        {
                            sw.WriteLine(entry);
                        }
                    }
                    sw.Flush();
                }
            }
            else
            {
                Dictionary<int, List<string>> entriesList = new Dictionary<int, List<string>>();
                var InputDirectoryPath = args[0];
                var OutputDirectoryPath = args[1];
                using (var sw = new StreamWriter(new FileStream(OutputDirectoryPath + "\\combinedMeasurementOutput.txt", FileMode.OpenOrCreate)))
                {

                    var files = Directory.EnumerateFiles(InputDirectoryPath, "*.txt");
                    sw.WriteLine("Threadcount,MilliSeconds,ThreadID");
                    sw.Flush();
                    var filecount = 0;
                    foreach (string file in files)
                    {
                        Console.WriteLine($"Reading file {++filecount} out of {files.Count()} + Name: {file}");
                        using (var sr = new StreamReader(new FileStream(file, FileMode.Open)))
                        {
                            sr.ReadLine();
                            while (!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                var entries = line.Split(':');
                                if (entries.Length >= 5 && int.Parse(entries[1]) % 50 == 0)
                                {
                                    int key = int.Parse(entries[1]);
                                    if (entriesList.ContainsKey(key))
                                    {
                                        entriesList[key].Add($"{entries[1]},{entries[3]},{entries[5]}");
                                    }
                                    else
                                    {
                                        entriesList.Add(key, new List<string>() { $"{entries[1]},{entries[3]},{entries[5]}" });
                                    }
                                }
                            }
                        }
                    }

                    foreach (var e in entriesList)
                    {
                        Console.WriteLine($"Writing {e.Key}s");
                        foreach (var entry in e.Value.Skip(e.Value.Count() / 2).Take(2000))
                        {
                            sw.WriteLine(entry);
                        }
                    }
                    sw.Flush();
                }
            }
        }
    }
}

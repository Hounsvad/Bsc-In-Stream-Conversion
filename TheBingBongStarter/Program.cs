using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TheBingBongStarter
{
    class Program
    {
        public static string helpMessage = "Usage:\n" +
        "\t-h, --help, h, help: returns this message\n" +
        "\tFirst argument: The path to the testclient exe\n" +
        "\tSecond argument: Passed to the test client as the first argument, \"auto\" if the test client should run automatically";
        static async Task Main(string[] args)
        {
            if(args.Length < 2)
            {
                if(args.Length < 1)
                {
                    Console.WriteLine(helpMessage);
                    return;
                }
                else
                {
                    args[0] = args[0].ToLower();
                    if(args[0] == "-h" || args[0] == "--help" || args[0] == "h" || args[0] == "help")
                    {
                        Console.WriteLine(helpMessage);
                        return;
                    }
                }

            }
            var counter = 0;
            while (true)
            {
                ProcessStartInfo p_info = new ProcessStartInfo();
                p_info.UseShellExecute = true;
                p_info.CreateNoWindow = false;
                //p_info.WindowStyle = ProcessWindowStyle.Minimized;
                p_info.FileName = args[0];
                p_info.Arguments = args[1] + " " + ++counter;
                Process.Start(p_info);
                Console.WriteLine("Ready to bing bong");
                Console.ReadLine();
                Console.WriteLine("Another One");
                //for(int i = 60; i > 0; i--)
                //{
                //    await Task.Delay(1000);
                //    Console.SetCursorPosition(0, Console.WindowHeight);
                //    Console.Write("T - " + i);
                //}
            }
        }
    }
}

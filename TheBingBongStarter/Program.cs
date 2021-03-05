using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TheBingBongStarter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                ProcessStartInfo p_info = new ProcessStartInfo();
                p_info.UseShellExecute = true;
                p_info.CreateNoWindow = false;
                p_info.WindowStyle = ProcessWindowStyle.Minimized;
                p_info.FileName = args[0];
                Process.Start(p_info);
                Console.WriteLine("Ready to bing bong");
                Console.ReadLine();
                for(int i = 60; i > 0; i--)
                {
                    await Task.Delay(1000);
                    Console.SetCursorPosition(0, Console.WindowHeight);
                    Console.Write("T - " + i);
                }
            }
        }
    }
}

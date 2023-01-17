using System;
using System.Diagnostics;

namespace AoC2022
{
    static class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            new Day22().Run();
            
            stopwatch.Stop();
            TimeSpan stopwatchElapsed = stopwatch.Elapsed;
            Console.WriteLine($"Completed in: {Convert.ToInt32(stopwatchElapsed.TotalMilliseconds)}ms");
        }
    }
}
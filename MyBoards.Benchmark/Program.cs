using BenchmarkDotNet.Running;

namespace MyBoards.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            BenchmarkRunner.Run<TrackingBenchmark>();
        }
    }
}
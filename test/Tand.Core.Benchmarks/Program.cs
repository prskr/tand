using BenchmarkDotNet.Running;

namespace Tand.Core.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<RationalTandBenchmerk>();
        }
    }
}
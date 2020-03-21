using System.Collections.Generic;
using Nuke.Common.Tooling;

namespace _build.benchmarks
{
    public static class DotNetBenchmarkExtensions
    {
        public static IReadOnlyCollection<Output> DotNetBenchmark(Configure<BenchmarkToolSettings> configurator)
        {
            return DotNetBenchmark(configurator(new BenchmarkToolSettings()));
        }
        
        public static IReadOnlyCollection<Output> DotNetBenchmark(BenchmarkToolSettings toolSettings = null)
        {
            toolSettings ??= new BenchmarkToolSettings();
            var process = ProcessTasks.StartProcess(toolSettings);
            process.AssertZeroExitCode();
            return process.Output;
        }
    }
}
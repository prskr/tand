using System;
using System.Collections.Generic;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

namespace _build.benchmarks
{
    public class BenchmarkToolSettings : ToolSettings
    {
        readonly IDictionary<string, string> _arguments;

        public BenchmarkToolSettings()
        {
            _arguments = new Dictionary<string, string>();
        }

        public override string ToolPath => base.ToolPath ?? DotNetTasks.DotNetPath;

        public string DllPath { get; private set; }

        public BenchmarkToolSettings WithDllPath(string path)
        {
            DllPath = path;
            return this;
        }

        public BenchmarkToolSettings WithFilter(string filter)
        {
            _arguments["filter"] = filter;
            return this;
        }

        public override Action<OutputType, string> CustomLogger => DotNetTasks.DotNetLogger;

        protected override Arguments ConfigureArguments(Arguments arguments)
        {
            arguments.Add("benchmark");
            arguments.Add(DllPath);
            foreach (var (k, v) in _arguments)
            {
                arguments.Add($"--{k} {{value}}", v);
            }

            return base.ConfigureArguments(arguments);
        }
    }
}
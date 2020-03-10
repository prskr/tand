using System;
using Xunit;
using Xunit.Abstractions;

namespace Tand.Core.Tests
{
    public class TandTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public TandTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void GenerateTand()
        {
            var tand = new Tand(new SampleResolver(_outputHelper));

            var sample = tand.DecorateWithTand<ITAndSample, TandSample>(new TandSample());
            var result = sample.LogMyParams("Hello, World", 42);
            _outputHelper.WriteLine($"Got result: {result}");
        }
    }
    
    
    public class SampleResolver : IDependencyResolver
    {
        private readonly ITestOutputHelper _outputHelper;

        public SampleResolver(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public ITandTarget<T> TargetOfType<T>(Type type)
        {
            return new LogTarget<T>(_outputHelper.WriteLine);
        }
    }

    public interface ITAndSample
    {
        [Tand(typeof(LogTarget<TandSample>))]
        int LogMyParams(string s, int i);
    }

    public class TandSample : ITAndSample
    {

        private int _counter;
        public string ContextSample { get; set; }
        
        public int LogMyParams(string s, int i)
        {
            return ++_counter;
        }
    }
}